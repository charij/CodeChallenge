namespace PlanetWars.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using PlanetWars.Common.Data;

    public class GamesController : Controller
    {
        private readonly GameManager _gameManager;

        public GamesController()
        {
            _gameManager = GameManager.Instance;
        }
        
        [HttpPost]
        [Route("logon")]
        public ActionResult Logon(Player profile)
        {
        }

        [HttpPost]
        [Route("join")]
        public ActionResult<GameDetails> Join(string gameId, Settings settings)
        {
            if (Guid.TryParse(gameId, out var id))
            {
                // attempt to join a game
            }
            else
            if (settings == null)
            {
                // create a new game
            }
            else 
            {
                // practice game against bot
            }
        }

        [HttpPost]
        [Route("submit")]
        public ActionResult<State> Move(Guid gameId, Command[] commands)
        {
        }
    }
}