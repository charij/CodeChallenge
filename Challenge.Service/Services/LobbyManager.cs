namespace PlanetWars.Server.Services
{
    using Challenge.Server.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using PlanetWars.Common;
    using PlanetWars.Common.Comm;
    using PlanetWars.Common.Data;
    using PlanetWars.Common.Data.Enum;
    using PlanetWars.Server.Data;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class LobbyManager
    {
        private readonly ILogger<LobbyManager> logger;
        private readonly GameDbContext dbContext;

        public LobbyManager(ILogger<LobbyManager> logger, GameDbContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        public async Task<Lobby[]> GetAllActiveLobbies()
        {
            var lobbies = await dbContext.Lobbies
                .Where(i => i.IsActive)
                .ToArrayAsync();

            return lobbies;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<Lobby> Create(string playerId, LobbySettings settings)
        {
            var player = await dbContext.Players.FirstOrDefaultAsync(i => i.Id == playerId);
            if (player == null)
            {
                logger.LogWarning($"Player {playerId} not found");
                throw new KeyNotFoundException($"Player {playerId} not found");
            }

            var lobby = player.Lobby = new Lobby
            {

            };

            dbContext.Lobbies.Add(lobby);
            dbContext.Players.Update(player);
            await dbContext.SaveChangesAsync();

            return lobby;
        }

        /// <summary>
        /// Joins a lobby
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="lobbyId"></param>
        /// <returns></returns>
        public async Task Join(string playerId, string lobbyId)
        {
            var isLobbyActive = await dbContext.Lobbies.AnyAsync(i => i.Id == lobbyId && i.IsActive);
            if (!isLobbyActive)
            {
                logger.LogWarning($"Lobby {lobbyId} does not exist");
                throw new KeyNotFoundException($"Lobby {lobbyId} does not exist");
            }

            var player = await dbContext.Players
                .Where(i => i.Id == playerId)
                .Include(i => i.Lobby)
                .FirstOrDefaultAsync();

            if (player == null)
            {
                logger.LogWarning($"Player {playerId} not found");
                throw new KeyNotFoundException($"Player {playerId} not found");
            }

            if (!player.Lobby.Players.Any())
            {
                dbContext.Lobbies.Remove(player.Lobby);
            }

            player.LobbyId = lobbyId;
            dbContext.Players.Update(player);
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task Leave(string playerId)
        {
            var player = await dbContext.Players
                .Where(i => i.Id == playerId)
                .Include(i => i.Lobby).ThenInclude(i => i.Players)
                .FirstOrDefaultAsync();

            if (player == null)
            {
                logger.LogWarning($"Player {playerId} not found");
                throw new KeyNotFoundException($"Player {playerId} not found");
            }

            player.LobbyId = null;
            dbContext.Players.Update(player);
            if (!player.Lobby.Players.Any())
            {
                dbContext.Lobbies.Remove(player.Lobby);
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<CommandResponse> Ready(string playerId, string lobbyId)
        {

        }
    }
}