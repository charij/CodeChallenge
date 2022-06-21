namespace Challenge.Server.Services
{
    using Challenge.Server.Data;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="lobbyTypes"></param>
        /// <param name="gameTypes"></param>
        /// <param name="isStarted"></param>
        /// <param name="isCompleted"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Lobby>> GetLobbies(
            int index = 0, 
            int count = 10, 
            string[] lobbyTypes = null, 
            string[] gameTypes = null, 
            bool? isStarted = null, 
            bool? isCompleted = null)
        {
            IQueryable<Lobby> lobbies = dbContext.Lobbies;

            if (lobbyTypes != null)
            {
                lobbies = lobbies.Where(i => lobbyTypes.Contains(i.LobbyType));
            }
            if (gameTypes != null)
            {
                lobbies = lobbies.Where(i => gameTypes.Contains(i.GameType));
            }
            if (isStarted.HasValue)
            {
                lobbies = lobbies.Where(i => i.StartedTime.HasValue == isCompleted.Value);
            }
            if (isCompleted.HasValue)
            {
                lobbies = lobbies.Where(i => i.CompletedTime.HasValue == isCompleted.Value);
            }

            return await lobbies
                .Skip(index)
                .Take(count)
                .ToArrayAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lobbyIds"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Lobby>> GetLobbies(string[] lobbyIds)
        {
            return await dbContext.Lobbies
                .Where(i => lobbyIds.Contains(i.Id))
                .ToArrayAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="lobbyType"></param>
        /// <param name="gameType"></param>
        /// <param name="maxPlayers"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public async Task<Lobby> Create(
            string playerId,
            string lobbyType,
            string gameType,
            int maxPlayers)
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

        public async Task Ready(string playerId, string lobbyId)
        {
            // return only when all lobby players are ready? then start game(s)
        }
    }
}