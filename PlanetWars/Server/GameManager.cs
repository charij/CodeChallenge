using PlanetWars.Shared;
using System.Collections.Generic;
using System.Linq;

namespace PlanetWars.Server
{
    public class GameManager
    {
        private static readonly GameManager _instance = new GameManager();
        public Dictionary<int, Game> Games { get; set; }
        private readonly static object GamesLock = new object();

        public static GameManager Instance
        {
            get { return _instance; }
        }

        private GameManager()
        {
            if (Games == null)
            {
                Games = new Dictionary<int, Game>();
            }
        }

        public Game GetNewGame()
        {
            var game = new Game();
            Games.Add(game.Id, game);
            return game;
        }

        public List<string> GetAllAuthTokens()
        {
            return Games.Values.SelectMany(g => g.AuthTokens.Keys).ToList();
        }

        public List<Game> GetAllActiveGames()
        {
            return Games.Values.Where(g => g.Running == true || g.GameOver == true).ToList();
        }

        public LogonResult Execute(LogonRequest request)
        {
            // check for waiting games and log players into that
            //lock (GamesLock)
            {
                var game = Games.Values.FirstOrDefault(g => g.Waiting);
                if (game == null)
                {
                    game = GetNewGame();
                    game.Waiting = true;
                    game.Start();
                    var logonResult = game.LogonPlayer(request.AgentName);
                    if (!logonResult.Success)
                    {
                        Games.Remove(game.Id);
                    }
                    return logonResult;
                }
                else
                {
                    var logonResult = game.LogonPlayer(request.AgentName);
                    if (logonResult.Success)
                    {
                        game.Waiting = false;
                        game.Start();
                    }
                    return logonResult;
                }
            }
        }

        public StatusResult Execute(StatusRequest request)
        {
            var game = Games[request.GameId];
            return game.GetStatus(request);
        }

        public MoveResult Execute(MoveRequest request)
        {
            var game = Games[request.GameId];
            return game.MoveFleet(request);
        }
    }
}
