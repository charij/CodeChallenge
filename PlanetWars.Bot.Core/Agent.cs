using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanetWars.Shared;
using PlanetWars.Bot.FSharp;

namespace CSharpAgentCore
{
    public class Agent : AgentBase
    {
        public Agent(string name, string endpoint) : base(name, endpoint){}

        /// <summary>
        /// Do your cool AI stuff
        /// </summary>
        /// <param name="gameState"></param>
        public override void Update(StatusResult gameState)
        {
            var moves = AgentFSharp.processGameState(MyId, gameState);

            foreach (var move in moves )
            {
                Console.WriteLine($"\n Target Planet: {move.TargetPlanetId} : ships {move.NumberOfShips}");
                SendFleet(move.SourcePlanetId, move.TargetPlanetId, move.NumberOfShips);
            }
        }
    }
}