namespace PlanetWars.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using PlanetWars.Common.Comm;
    using PlanetWars.Common.Data;
    using PlanetWars.Server.Services;
    using System.Threading.Tasks;

    [Authorize]
    [Controller]
    public class GameController : Controller
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly LobbyManager lobbyManager;

        public GameController(IHttpContextAccessor contextAccessor, LobbyManager lobbyManager)
        {
            this.contextAccessor = contextAccessor;
            this.lobbyManager = lobbyManager;
        }
        
        [HttpPost("Join")]
        public async Task<ActionResult<Game>> JoinGame()
        {
            if (!Request.Cookies.TryGetValue("playerId", out var playerId)) return Unauthorized();
            if (!Request.Cookies.TryGetValue("lobbyId",  out var lobbyId))  return Unauthorized();

            var details = await lobbyManager.JoinLobby(playerId, lobbyId);
            var cookieOptions = new CookieOptions
            {
                Secure = true,
                IsEssential = true
            };

            Response.Cookies.Append("GameId", $"{details.Id}", cookieOptions);

            return Ok(details);
        }

        [HttpPost("Submit")]
        public async Task<ActionResult<CommandResponse>> Submit(CommandRequest[] commands)
        {
            if (!Request.Cookies.TryGetValue("playerId", out var playerId)) return Unauthorized();
            if (!Request.Cookies.TryGetValue("gameId",   out var gameId))   return Unauthorized();
             
            var response = await lobbyManager.SubmitMove(playerId, gameId, commands);

            if (response.State.IsGameOver) 
            {
                Response.Cookies.Delete("gameId");
            }

            return Ok(response);
        }
    }
}