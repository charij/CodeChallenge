using System;
using System.Collections.Generic;
using System.Linq;
using PlanetWars.Shared;

namespace CSharpAgent
{
    public class Agent : AgentBase
    {
        public Agent(string name, string endpoint) : base(name, endpoint) { }

        public int Value(StatusResult gs, Planet planet, IEnumerable<Planet> myPlanets)
        {
            return planet.OwnerId == OppId
                ? ((200 - gs.CurrentTurn) * planet.GrowthRate * 2)
                : ((200 - gs.CurrentTurn) * planet.GrowthRate) - planet.NumberOfShips;
        }

        public int ShipsNeeded(StatusResult gs, Planet source, Planet target)
        {
            var fleetOffset = gs.Fleets.Where(i => i.OwnerId == OppId && i.SourcePlanetId == target.Id).Sum(i => i.NumberOfShips) 
                            - gs.Fleets.Where(i => i.OwnerId == MyId && i.SourcePlanetId == target.Id).Sum(i => i.NumberOfShips);

            return target.OwnerId == OppId
                ? (target.NumberOfShips + (target.GrowthRate * (int)Math.Ceiling(source.Position.Distance(target.Position))) + 1) - fleetOffset
                : (target.NumberOfShips) - fleetOffset;
        }

        public override void Update(StatusResult gs)
        {
            if (gs.CurrentTurn == 0)
            {
                Console.WriteLine($"Turn {gs.CurrentTurn}\t Biding my time!");
                return;
            }

            if (gs.Planets.All(i => i.OwnerId == MyId))
            {
                Console.WriteLine($"Turn {gs.CurrentTurn}\t We won!");
                return;
            }

            var myPlanets = gs.Planets.Where(i => i.OwnerId == MyId);
            var notMyPlanets = gs.Planets.Where(i => i.OwnerId != MyId);
            var oppPlanets = gs.Planets.Where(i => i.OwnerId == OppId);
            var neutralPlanets = gs.Planets.Where(i => i.OwnerId == -1);

            foreach (var target in notMyPlanets.OrderByDescending(i => Value(gs, i, myPlanets)))
            {
                foreach (var source in myPlanets.OrderBy(i => i.Position.Distance(target.Position)))
                {
                    SendFleet(source.Id, target.Id, Math.Min(ShipsNeeded(gs, source, target), source.NumberOfShips));
                }
            }
        }
    }
}