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

            // find the first planet we don't own
            var targetPlanets = new List<int>();

            var neutralPlanets = gameState.Planets.Where(p => p.OwnerId == -1) ;
            var opponentPlanets = gameState.Planets.Where(p => p.OwnerId != MyId & p.OwnerId != -1);
            var opponentGrowthRate = opponentPlanets.Select(x => x.GrowthRate).Sum();

    
            // send half rounded down of our ships from each planet we do own
            foreach (var planet in gameState.Planets.Where(p => p.OwnerId == MyId))
            {
                var untragetedNeutralPlanets = neutralPlanets.Where(x => !targetPlanets.Contains(x.Id)).ToList();
                Planet targetPlanet;
                if(untragetedNeutralPlanets.Count > 0)
                {
                    targetPlanet =untragetedNeutralPlanets.OrderBy(x => GetPlanetDistance(x, planet)).FirstOrDefault();
                }
                else
                {
                   targetPlanet  = neutralPlanets.OrderBy(x => GetPlanetDistance(x, planet)).FirstOrDefault();
                }                

                if (targetPlanet == null)
                {
                    targetPlanet = opponentPlanets.OrderBy(x => GetPlanetDistance(x, planet)).FirstOrDefault();
                    
                }

                targetPlanets.Add(targetPlanet.Id);

                var ships = (int)Math.Floor(planet.NumberOfShips / 2.0);
                if (ships > 0)
                {
                    SendFleet(planet.Id, targetPlanet.Id, ships);
                }
            }
        }
        private double GetPlanetDistance(Planet destination, Planet source)
        {

            return Math.Ceiling(Math.Sqrt(
            Math.Pow(destination.Position.X - source.Position.X, 2) +
            Math.Pow(destination.Position.Y - source.Position.Y, 2)));
        }
    }
   
    
}