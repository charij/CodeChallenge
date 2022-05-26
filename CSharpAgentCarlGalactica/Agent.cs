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
        public int MIN_DEFENCE = 10;
        public int MIN_FLEET = 10;

        public Agent(string name, string endpoint) : base(name, endpoint) { }

        /// <summary>
        /// Do your cool AI stuff
        /// </summary>
        /// <param name="gameState"></param>
        public override void Update(StatusResult gameState)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] Current Turn: {gameState.CurrentTurn}");
            Console.WriteLine($"Owned Planets: {string.Join(", ", gameState.Planets.Where(p => p.OwnerId == MyId).Select(p => p.Id))}");

            Planet targetPlanet = null;

            // keep hitting current target _until we have enough ships on the way_
            if (currentTargetId >= 0)
            {
                var planet = gameState.Planets.FirstOrDefault(p => p.Id == currentTargetId);
                if (planet != null && planet.OwnerId != MyId && MyShipsOnTheWay(gameState, planet) < planet.NumberOfShips + 1)
                {
                    targetPlanet = planet;
                }
            }

            if (targetPlanet == null)
            {   // otherwise pick the _nearest_ planet we don't own that doesn't have enough ships on the way to it

                var nearestOtherPlanet = NearestToMeNotEnoughShips(gameState);
                if (nearestOtherPlanet == null) return;

                targetPlanet = nearestOtherPlanet;
                currentTargetId = targetPlanet.Id;
            }

            Console.WriteLine($"Target Planet: {targetPlanet.Id}:{targetPlanet.NumberOfShips}");

            // send half rounded down of our ships from each planet we do own BUT keep 10 for defence
            foreach (var planet in gameState.Planets.Where(p => p.OwnerId == MyId))
            {
                var halfShips = (int)Math.Floor(planet.NumberOfShips / 2.0);
                var ships = Math.Max(halfShips, planet.NumberOfShips-MIN_DEFENCE);
                Console.WriteLine($"Ships {planet.NumberOfShips}, send {ships}");
                if (ships > MIN_FLEET)
                {
                    SendFleet(planet.Id, targetPlanet.Id, ships);
                }
            }
        }

        public Planet NearestToMyFirst(StatusResult gameState)
        {
            var myFirstPlanet = gameState.Planets.FirstOrDefault(p => p.OwnerId == MyId);
            if (myFirstPlanet == null) return null;

            var nearestOtherPlanet = NotMyPlanets(gameState).OrderBy(p => p.Position.Distance(myFirstPlanet.Position)).FirstOrDefault();

            return nearestOtherPlanet;
        }

        public double TotalDistance(Planet target, IEnumerable<Planet> planets)
        {
            return planets.Sum(p => p.Position.Distance(target.Position));
        }

        public Planet NearestToMeFrom(StatusResult gameState, IEnumerable<Planet> planets)
        {
            var myPlanets = gameState.Planets.Where(p => p.OwnerId == MyId);
            if (!myPlanets.Any()) return null;

            var nearestOtherPlanet = planets.OrderBy(p => TotalDistance(p, myPlanets)).FirstOrDefault();

            return nearestOtherPlanet;
        }

        public Planet NearestToMe(StatusResult gameState)
        {
            var nearestOtherPlanet = NearestToMeFrom(gameState, NotMyPlanets(gameState));

            return nearestOtherPlanet;
        }

        private IEnumerable<Planet> NotMyPlanets(StatusResult gameState)
        {
            return gameState.Planets.Where(p => p.OwnerId != MyId);
        }

        public Planet NearestToMeNotEnoughShips(StatusResult gameState)
        {
            var others = NotMyPlanets(gameState);
            var needMore = others.Where(p => MyShipsOnTheWay(gameState, p) < p.NumberOfShips+1);

            var nearestOtherPlanet = NearestToMeFrom(gameState, needMore);

            var test = NearestToMe(gameState);
            if (test.Id != nearestOtherPlanet.Id)
                Console.WriteLine($"Different {test.Id} vs {nearestOtherPlanet.Id}");
            else
                Console.WriteLine($"Same");

            return nearestOtherPlanet;
        }

        public int MyShipsOnTheWay(StatusResult gameState, Planet planet)
        {
            var myFleets = gameState.Fleets.Where(f => f.DestinationPlanetId == planet.Id && f.OwnerId == MyId);
            var numShips = myFleets.Sum(f => f.NumberOfShips);
            Console.WriteLine($"ships {numShips} to {planet.Id} which has {planet.NumberOfShips}");
            return numShips;
        }
    }
}

// "First neutral planet" - loses to basic "attack first enemy planet"
// "neutral planet with least ships" - loses to basic "attack first enemy planet"