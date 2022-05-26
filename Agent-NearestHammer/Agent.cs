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
            {   // otherwise pick the _nearest_ planet we don't own

                var nearestOtherPlanet = NearestToMe(gameState);
                if (nearestOtherPlanet == null) return;

                targetPlanet = nearestOtherPlanet;
                currentTargetId = targetPlanet.Id;
            }

            Console.WriteLine($"Target Planet: {targetPlanet.Id}:{targetPlanet.NumberOfShips}");                       

            // send our ships from each planet we do own
            foreach (var planet in gameState.Planets.Where(p => p.OwnerId == MyId))
            {
                var ships = planet.NumberOfShips - 1;
                if (ships > 0)
                {
                    SendFleet(planet.Id, targetPlanet.Id, ships);
                }
            }
        }

        public Planet NearestToMyFirst(StatusResult gameState)
        {
            var myFirstPlanet = gameState.Planets.FirstOrDefault(p => p.OwnerId == MyId);
            if (myFirstPlanet == null) return null;

            var nearestOtherPlanet = gameState.Planets.Where(p => p.OwnerId != MyId).OrderBy(p => p.Position.Distance(myFirstPlanet.Position)).FirstOrDefault();

            return nearestOtherPlanet;
        }

        public double TotalDistance(Planet target, IEnumerable<Planet> planets)
        {
            return planets.Sum(p => p.Position.Distance(target.Position));
        }

        public Planet NearestToMe(StatusResult gameState)
        {
            var myPlanets = gameState.Planets.Where(p => p.OwnerId == MyId);
            if (!myPlanets.Any()) return null;

            var nearestOtherPlanet = gameState.Planets.Where(p => p.OwnerId != MyId).OrderBy(p => TotalDistance(p, myPlanets)).FirstOrDefault();

            return nearestOtherPlanet;
        }
    }
}