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

        public int Value(StatusResult gs, Planet planet, Planet[] OurPlanets)
        {
            return planet.OwnerId == 0
                ? ((200 - gs.CurrentTurn) * planet.GrowthRate) - planet.NumberOfShips
                : ((200 - gs.CurrentTurn) * planet.GrowthRate) * 2;
        }

        private StatusResult previousState;

        public override void Update(StatusResult gs)
        {
            if (gs.CurrentTurn == 0) // Bide time
            {
                Console.WriteLine($"Turn {gs.CurrentTurn}\t Biding my time!");
                previousState = gs;
                return;
            }
            
            if (gs.Planets.All(i => i.OwnerId == MyId))
            {
                Console.WriteLine($"Turn {gs.CurrentTurn}\t We won!");
                return;
            }

            // get opponents moves & counter based on value
            var previousFleets = previousState.Fleets.Select(i => i.Id);
            var opponentMoves = gs.Fleets.Where(i => !previousFleets.Contains(i.Id));
            foreach (var move in opponentMoves)
            { 
                // add scheduled moves
            }

            // move all remaining ships to border via MST
            var myPlanets = gs.Planets.Where(i => i.OwnerId == MyId);
            foreach (var planet in myPlanets)
            {
                
            }

            previousState = gs;


            if (targetPlanet == null) return; // WE OWN IT ALLLLLLLLL

            Console.WriteLine($"Target Planet: {targetPlanet.Id}:{targetPlanet.NumberOfShips}");                       

            // send half rounded down of our ships from each planet we do own
            foreach (var planet in gs.Planets.Where(p => p.OwnerId == MyId))
            {
                if (planet.NumberOfShips > 0)
                {
                    SendFleet(planet.Id, targetPlanet.Id, planet.NumberOfShips);
                }
            }
        }
    }
}
