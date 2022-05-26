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
            var ourPlanets = gameState.Planets.Where(p => p.OwnerId == MyId);
            var enemyId = MyId == 1 ? 2 : 1;

            var underAttack = gameState.Fleets.Where(x => x.OwnerId == enemyId && ourPlanets
                    .Where(y => y.Id == x.DestinationPlanetId).Count() > 0)
                    .Select(x => x.DestinationPlanetId);

            // send half rounded down of our ships from each planet we do own
            foreach (var planet in gameState.Planets.Where(p => p.OwnerId == MyId))
            {
                var untragetedNeutralPlanets = neutralPlanets.Where(x => !targetPlanets.Contains(x.Id)).ToList();
                Planet targetPlanet;
                var ships = (int)Math.Floor(planet.NumberOfShips / 2.0);
                if (underAttack.Contains(planet.Id))
                {
                    ships = (int)Math.Floor(planet.NumberOfShips / 4.0);
                }
                else
                {
                    ships = (int)Math.Floor(planet.NumberOfShips / 1.25);
                }

                if(untragetedNeutralPlanets.Count > 0)
                {
                    targetPlanet =untragetedNeutralPlanets.OrderBy(x => GetPlanetDistance(x, planet)).FirstOrDefault();
                }
                else
                {
                   targetPlanet  = opponentPlanets.OrderBy(x => GetPlanetDistance(x, planet)).FirstOrDefault();
                }  
           

                targetPlanets.Add(targetPlanet.Id);

                
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