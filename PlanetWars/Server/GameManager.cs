﻿using PlanetWars.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetWars.Server
{
    public class GameManager
    {
        private static readonly GameManager _instance = new GameManager();
        public Dictionary<int, Game> Games { get; set; }

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

        public Game GetWaitingGame()
        {
            return Games.Values.FirstOrDefault(g => g.Waiting);
        }

        public StatusResult GetGameStatus(int gameId)
        {
            if (!Games.ContainsKey(gameId)) {
                return null;
            }
            var game = Games[gameId];            
            var result = game.GetStatus(null);
            return result;
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
            var game = GetWaitingGame();
            if (game != null)
            {
                game.Waiting = false;
                return game.LogonPlayer(request.AgentName);
            }
            else
            {
                game = GetNewGame();
                game.Waiting = true;
                game.Start();
                //game.StartDemoAgent("Demo");
                return game.LogonPlayer(request.AgentName);
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
