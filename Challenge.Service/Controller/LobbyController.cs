namespace Challenge.Server.Controllers
{
    using Challenge.Server.Data;
    using Challenge.Server.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize]
    [Controller]
    public class LobbyController : Controller
    {
        private readonly ILogger<LobbyController> logger;
        private readonly LobbyManager lobbyManager;

        public LobbyController(ILogger<LobbyController> logger, LobbyManager lobbyManager)
        {
            this.logger = logger;
            this.lobbyManager = lobbyManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lobby>>> Get(
            [FromBody] int index, 
            [FromBody] int count,
            [FromBody] string[] lobbyTypes,
            [FromBody] string[] gameTypes,
            [FromBody] bool? isStarted,
            [FromBody] bool? isCompleted)
        {
            var lobbies = await lobbyManager.GetLobbies(index, count, lobbyTypes, gameTypes, isStarted, isCompleted);

            return lobbies.Any()
                ? Ok(lobbies)
                : NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lobby>>> Get(
            [FromBody] string[] lobbyIds)
        {
            var lobbies = await lobbyManager.GetLobbies(lobbyIds);

            return lobbies.Any()
                ? Ok(lobbies)
                : NoContent();
        }

        [HttpPost("Create")]
        public async Task<ActionResult<Lobby>> Create(
            [FromBody] string lobbyType,
            [FromBody] string gameType,
            [FromBody] int maxPlayers)
        {
            if (!Request.Cookies.TryGetValue("playerId", out var playerId))
            {
                return Unauthorized();
            }

            var lobby = await lobbyManager.Create(playerId, lobbyType, gameType, maxPlayers);

            return Ok(lobby);
        }

        [HttpPost("Join")]
        public async Task<ActionResult> Join(
             [Required] string lobbyId)
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