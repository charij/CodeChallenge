using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlanetWars.Shared;

namespace CSharpAgent
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
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] Current Turn: {gameState.CurrentTurn}");
            Console.WriteLine($"Owned Planets: {string.Join(", ", gameState.Planets.Where(p => p.OwnerId == MyId).Select(p =>  p.Id))}");

            // find the neutral planet with least ships
            var neutralPlanet = gameState.Planets.Where(p => p.OwnerId == -1).OrderBy(p => p.Size).FirstOrDefault();

            // find the first planet we don't own
            var otherPlanet = gameState.Planets.FirstOrDefault(p => p.OwnerId != MyId);
            if (otherPlanet == null) return;

            var targetPlanet = neutralPlanet ?? otherPlanet;
            if (otherPlanet == null) return;

            Console.WriteLine($"Target Planet: {targetPlanet.Id}:{targetPlanet.NumberOfShips}");                       

            // send half rounded down of our ships from each planet we do own
            foreach (var planet in gameState.Planets.Where(p => p.OwnerId == MyId))
            {
                var ships = (int)Math.Floor(planet.NumberOfShips / 2.0);
                if (ships > 0)
                {
                    SendFleet(planet.Id, targetPlanet.Id, ships);
                }
            }
        }
    }
}

// "First neutral planet" - loses to basic "attack first enemy planet"
// "neutral planet with least ships" - loses to basic "attack first enemy planet"