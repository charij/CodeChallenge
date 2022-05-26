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
        public int currentTargetId = -1;

        public Agent(string name, string endpoint) : base(name, endpoint){}

        /// <summary>
        /// Do your cool AI stuff
        /// </summary>
        /// <param name="gameState"></param>
        public override void Update(StatusResult gameState)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] Current Turn: {gameState.CurrentTurn}");
            Console.WriteLine($"Owned Planets: {string.Join(", ", gameState.Planets.Where(p => p.OwnerId == MyId).Select(p =>  p.Id))}");

            Planet targetPlanet = null;

            // keep hitting current target until own it
            if (currentTargetId >= 0)
            { 
                var planet = gameState.Planets.FirstOrDefault(p => p.Id==currentTargetId);
                if (planet != null && planet.OwnerId != MyId)
                    targetPlanet = planet;
            }

            if (targetPlanet == null)
            {   // otherwise pick a _random_ planet we don't own
                var otherPlanets = gameState.Planets.Where(p => p.OwnerId != MyId).ToList();
                if (!otherPlanets.Any()) return;

                int iTarget = new Random().Next(otherPlanets.Count());
                targetPlanet = otherPlanets[iTarget];

                currentTargetId = targetPlanet.Id;
            }

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