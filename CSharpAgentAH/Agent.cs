using PlanetWars.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpAgent
{

    public enum PlanetStatus
    {
        Neutral,
        Friendly,
        Hostile
    }

    public class Agent : AgentBase
    {
        private const int MaxTurns = 200;

        public Agent(string name, string endpoint) : base(name, endpoint){}

        public override void Update(StatusResult gameState)
        {
            var allPlanets = gameState.Planets.Select(x => new AgentPlanet(x, MyId, gameState));
            foreach(var planet in allPlanets)
            {
                planet.CalculateValue();
                planet.SetShipsRequiredForDefence();
                planet.SetShipsRequiredToConquer();
                planet.CalculateExcessShips(0);
            }

            var friendlyPlanets = allPlanets
                //.Where(x => x.ExcessShips > 0)
                .Where(x => x.Status == PlanetStatus.Friendly)
                .ToList();

            // first protect our planets
            var helpTo = allPlanets
                .Where(x => x.Status == PlanetStatus.Friendly && x.ShipsRequiredForDefence < 0)
                .OrderBy(x => x.ShipsRequiredForDefence)
                .ToList();

            foreach (var planet in helpTo)
            {
                SendShips(friendlyPlanets, planet, Math.Abs(planet.ShipsRequiredForDefence));
            }

            // after protecting us, attack other planets
            var attackTargets = allPlanets
                .Where(x => x.Status != PlanetStatus.Friendly)
                .OrderByDescending(x => Math.Abs(x.Value.Saldo));

            foreach (var planet in attackTargets)
            {
                SendShips(friendlyPlanets, planet, planet.ShipsRequiredToConquer);
            }
        }

        private void SendShips(List<AgentPlanet> ourPlanets, AgentPlanet target, int requiredShips)
        {
            var sendFrom = ourPlanets
                    .Where(x => x.ExcessShips > 0)
                    .OrderBy(x => x.GetDistanceTo(target));

            foreach (var from in sendFrom)
            {
                if (requiredShips <= 0) continue;

                var amount = Math.Min(requiredShips, from.ExcessShips);
                SendFleet(from.Id, target.Id, amount);
                from.CalculateExcessShips(amount); // defo not a bug?

                requiredShips -= amount;
            }
        }
    }
}