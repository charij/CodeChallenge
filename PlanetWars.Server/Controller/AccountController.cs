namespace PlanetWars.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using PlanetWars.Common.Comm;
    using PlanetWars.Common.Data;
    using PlanetWars.Server.Services;
    using System;
    using System.Threading.Tasks;

    [Authorize]
    [Controller]
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly LobbyManager lobbyManager;

        public AccountController(IHttpContextAccessor contextAccessor, LobbyManager lobbyManager)
        {
            this.contextAccessor = contextAccessor;
            this.lobbyManager = lobbyManager;
        }
        
        [AllowAnonymous]
        [HttpPost("logon")]
        public ActionResult<string> Logon(string name)
        {
            var playerId = Guid.NewGuid();
            var cookieOptions = new CookieOptions
            {
                Secure = true,
                IsEssential = true                
            };

            Response.Cookies.Append("playerId", $"{playerId}", cookieOptions);
            Response.Cookies.Append("playerName", name, cookieOptions);

            return Ok($"{playerId}");
        }
    }
}