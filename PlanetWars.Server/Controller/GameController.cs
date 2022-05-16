namespace PlanetWars.Controllers
{
    using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PlanetWars.Common.Data;

    [Controller]
    public class GameController : Controller
    {
        private readonly GameManager _gameManager;

        public GameController()
        {
            _gameManager = GameManager.Instance;
        }
        
        [AllowAnonymous]
        [HttpPost("logon")]
        public ActionResult Logon(Player profile)
        {
        }

        [HttpPost("join")]
        public ActionResult<GameDetails> Join(string gameId, Settings settings)
        {
            if (!string.IsNullOrEmpty(gameId) && Guid.TryParse(gameId, out var id))
            {
                // attempt to join a game / league
            }
            else
            if (settings == null)
            {
                // create a new game
            }
            else 
            {
                // join a random lobby game
            }
        }

        [HttpPost("submit")]
        public ActionResult<State> Move(Guid gameId, Command[] commands)
        {
        }
    }
}