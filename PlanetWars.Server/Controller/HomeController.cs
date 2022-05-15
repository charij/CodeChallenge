namespace PlanetWars.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Game(int id)
        {
            if (GameManager.Instance.Games.ContainsKey(id))
            {

                var gameModel = new GameSession()
                {
                    GameId = id,
                    Players = GameManager.Instance.Games[id].Players.ToDictionary(kv => kv.Value.Id, kv => kv.Value.PlayerName)
                };
                return View(gameModel);
            }
            throw new HttpException(404, "Game not found");
        }
    }
}