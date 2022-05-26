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
        public Agent(string name, string endpoint) : base(name, endpoint) { }

        /// <summary>
        /// Do your cool AI stuff
        /// </summary>
        /// <param name="gameState"></param>
        public override void Update(StatusResult gameState)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] Current Turn: {gameState.CurrentTurn}");
            Console.WriteLine($"Owned Planets: {string.Join(", ", gameState.Planets.Where(p => p.OwnerId == MyId).Select(p => p.Id))}");

            var freePlanets = gameState.Planets.Where(p => p.OwnerId == -1);
            var occupiedPlanets = gameState.Planets.Where(p => p.OwnerId != -1 && p.OwnerId != MyId);

            var turnsLeft = 200 - gameState.CurrentTurn;

            var targetPlanet = freePlanets.FirstOrDefault(); ;
            if (targetPlanet == null)
                targetPlanet = occupiedPlanets.FirstOrDefault();
                if (targetPlanet == null) return;

            var planetsWeOwn = gameState.Planets.Where(p => p.OwnerId == MyId).OrderByDescending(p => p.NumberOfShips);

            foreach (var planet in planetsWeOwn)
            {
                // ATTACK
                var cost = (targetPlanet.NumberOfShips + ((int)Math.Floor(planet.Position.Distance(targetPlanet.Position)) * targetPlanet.GrowthRate) )+ 1;

                var ships = (int)Math.Floor(planet.NumberOfShips / 2.0);
                if (ships > 0)
                {
                    SendFleet(planet.Id, targetPlanet.Id, ships);
                }

                //DEFENSE
                var allies = planetsWeOwn.OrderBy(x => x.Position.Distance(planet.Position));

                var totalShipsComingForUs = gameState.Fleets.Where(x => x.OwnerId != MyId).Where(x => x.DestinationPlanetId == planet.Id).Select(x => x.NumberOfShips).Sum();

                var differential = totalShipsComingForUs - planet.NumberOfShips;

                foreach (var ally in allies)
                {
                    if (differential > 0)

                    {
                        Console.WriteLine("Defensive Manuavres");
                        var aidingShips = (int)Math.Floor(ally.NumberOfShips / 4.0);

                        // Send backup
                        SendFleet(ally.Id, planet.Id, aidingShips);
                        differential = differential - aidingShips;
                    }
                }

            }
        }
    }
}