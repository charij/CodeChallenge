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

            var freePlanets = GetFreePlanets(gameState.Planets);
            var occupiedPlanets = GetOccupiedPlanets(gameState.Planets, MyId);

            var targetPlanet = occupiedPlanets.FirstOrDefault();
            if (targetPlanet == null)
                targetPlanet = freePlanets.FirstOrDefault();
            if (targetPlanet == null) return;

            //Console.WriteLine($"Target Planet: {targetPlanet.Id}:{targetPlanet.NumberOfShips}");

            foreach (var planet in gameState.Planets.Where(p => p.OwnerId == MyId))
            {
                //var closestFree = FindClosestPlanet(planet, freePlanets);
                //var closestOccupied = FindClosestPlanet(planet, occupiedPlanets);

                //var targetPlanet = closestFree;
                var ships = (int)Math.Floor(planet.NumberOfShips / 2.0);
                if (ships > 0)
                {
                    SendFleet(planet.Id, targetPlanet.Id, ships);
                }
            }
        }

        public static List<Planet> GetFreePlanets(List<Planet> planets)
        {
            var freePlanets = planets.Where(p => p.OwnerId == -1);
            if (freePlanets != null)
            {
                var orderedFreePlanets = freePlanets.OrderByDescending(fp => fp.GrowthRate / fp.NumberOfShips);
                return orderedFreePlanets.ToList();
            }

            return new List<Planet>(); 
        }

        public static List<Planet> GetOccupiedPlanets(List<Planet> planets, int myId)
        {
            var occupiedPlanets = planets.Where(p => p.OwnerId != myId);
            if (occupiedPlanets != null)
            {
                var orderedOP = occupiedPlanets.OrderByDescending(op => op.GrowthRate / op.NumberOfShips);

                return orderedOP.ToList();
            }
            return new List<Planet>();
        }

        public static Planet FindClosestPlanet(Planet sourcePlanet,  List<Planet> destinationPlanets)
        {
            var planetsByDistance = destinationPlanets.OrderBy(dp => sourcePlanet.Position.Distance(dp.Position));
            return planetsByDistance.FirstOrDefault();
        }
    }
}