namespace Challenge.Server.Controllers
{
    using Challenge.Server.Data;
    using Challenge.Server.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize]
    [Controller]
    public class LobbyController : Controller
    {
        private readonly ILogger<LobbyController> logger;
        private readonly GameDbContext dbContext;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly LobbyManager lobbyManager;

        public LobbyController(ILogger<LobbyController> logger, GameDbContext dbContext, IHttpContextAccessor contextAccessor, LobbyManager lobbyManager)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.contextAccessor = contextAccessor;
            this.lobbyManager = lobbyManager;
        }

        [HttpGet("All")]
        public async Task<ActionResult<Lobby[]>> GetAllActiveLobbies()
        {
            var lobbies = await dbContext.Lobbies
                .Where(i => i.IsActive)
                .ToArrayAsync();

            return lobbies.Any()
                ? Ok(lobbies)
                : NoContent();
        }

        [HttpPost("Create")]
        public async Task<ActionResult<Lobby>> Create(LobbySettings settings)
        {
            if (!Request.Cookies.TryGetValue("playerId", out var playerId))
            {
                return Unauthorized();
            }

            var lobby = await lobbyManager.Create(playerId, settings);
            Response.Cookies.Append("lobbyId", $"{lobby.Id}", new CookieOptions { Secure = true });
            return Ok(lobby);
        }

        [HttpPost("Join")]
        public async Task<ActionResult> Join([Required]string lobbyId)
        {
            if (!Request.Cookies.TryGetValue("playerId", out var playerId))
            {
                return Unauthorized();
            }

            await lobbyManager.Join(playerId, lobbyId);
            Response.Cookies.Append("lobbyId", $"{lobbyId}", new CookieOptions { Secure = true });
            return Ok();
        }

        [HttpPost("Leave")]
        public async Task<ActionResult> Leave()
        {
            if (!Request.Cookies.TryGetValue("playerId", out var playerId))
            {
                return Unauthorized();
            }

            await lobbyManager.Leave(playerId);
            Response.Cookies.Delete("lobbyId");
            return Ok();
        }

        [HttpPost("Ready")]
        public async Task<ActionResult<Game>> Ready([Required]string lobbyId)
        {
            if (!Request.Cookies.TryGetValue("playerId", out var playerId))
            {
                return Unauthorized();
            }

            var game = await lobbyManager.Ready(playerId, lobbyId);
            Response.Cookies.Append("GameId", $"{game.Id}", new CookieOptions { Secure = true });
            return Ok(game);
        }
    }
}