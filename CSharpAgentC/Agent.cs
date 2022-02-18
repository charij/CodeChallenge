using System;
using System.Linq;
using PlanetWars.Extensions;
using PlanetWars.Shared;

namespace CSharpAgent
{
    public class Agent : AgentBase
    {
        public Agent(string name, string endpoint) : base(name, endpoint) { }

        public override void Update(StatusResult gs)
        {
            if (gs.CurrentTurn == 0)
            {
                Console.WriteLine("Biding my time!");
                return;
            }

            foreach (var target in gs.Planets.OrderByDescending(i => Value(gs, i)))
            foreach (var source in gs.MyPlanets(MyId).OrderBy(i => i.Distance(target)))
            {
                SendFleet(source, target, ShipsNeeded(gs, source, target));
            }
        }

        public int Value(StatusResult gs, Planet planet)
        {
            var turnsToCapture = 0;
            var axisShips = gs.Fleets.Where(i => i.SourcePlanetId == planet.Id).Where(i => i.OwnerId != MyId).Sum(i => i.NumberOfShips);
            var allyShips = gs.Fleets.Where(i => i.SourcePlanetId == planet.Id).Where(i => i.OwnerId == MyId).Sum(i => i.NumberOfShips);
            var shipsNeeded = planet.OwnerId == -1
                ? planet.NumberOfShips + axisShips - allyShips
                : planet.OwnerId == MyId
                    ? axisShips - (planet.NumberOfShips + allyShips)
                    : planet.NumberOfShips + axisShips + (turnsToCapture * planet.GrowthRate) - allyShips;

            var value = ((200 - (gs.CurrentTurn + turnsToCapture)) * planet.GrowthRate) - shipsNeeded;

            return planet.OwnerId > -1
                ? value * 2
                : value;
        }
        public int ShipsNeeded(StatusResult gs, Planet source, Planet target)
        {
            var turnsToCapture = 1;
            var axisShips = gs.Fleets.Where(i => i.OwnerId == OppId).Where(i => i.DestinationPlanetId == target.Id).Sum(i => i.NumberOfShips);
            var allyShips = gs.Fleets.Where(i => i.OwnerId == MyId).Where(i => i.DestinationPlanetId == target.Id).Sum(i => i.NumberOfShips)
                         + _pendingMoveRequests.Where(i => i.DestinationPlanetId == target.Id).Sum(i => i.NumberOfShips);

            var shipsNeeded = target.OwnerId == -1 
                    ? target.NumberOfShips - (axisShips + allyShips) + 1
                : target.OwnerId == OppId
                    ? target.NumberOfShips + axisShips - allyShips + (target.GrowthRate * source.Distance(target)) + 1
                    : axisShips - target.NumberOfShips - allyShips - (target.GrowthRate * turnsToCapture);

            var shipsAvailable = source.NumberOfShips 
                - _pendingMoveRequests.Where(i => i.SourcePlanetId == source.Id)?.Sum(i => i.NumberOfShips) ?? 0 
                + gs.Fleets.Where(i => i.OwnerId == MyId).Where(i => i.DestinationPlanetId == source.Id)?.Sum(i => i.NumberOfShips) ?? 0
                - gs.Fleets.Where(i => i.OwnerId == OppId).Where(i => i.DestinationPlanetId == source.Id)?.Sum(i => i.NumberOfShips) ?? 0
                + gs.Fleets.Where(i => i.OwnerId == OppId).Where(i => i.DestinationPlanetId == source.Id).Select(i => i.NumberOfTurnsToDestination)?.Min() ?? 0 * source.GrowthRate
                ;

            return Math.Min(shipsAvailable, shipsNeeded);
        }
    }
}