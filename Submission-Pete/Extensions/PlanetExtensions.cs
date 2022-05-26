namespace PlanetWars.Extensions
{
    using PlanetWars.Shared;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class PlanetExtensions
    {
        public static int Distance(this Planet a, Planet b)
        {
            return (int)Math.Ceiling(a.Position.Distance(b.Position));
        }

        public static IEnumerable<Planet> MyPlanets(this StatusResult gs, int myId)
        {
            return gs.Planets.Where(i => i.OwnerId == myId);
        }

        public static IEnumerable<Planet> OpponentPlanets(this StatusResult gs, int opponentId)
        {
            return gs.Planets.Where(i => i.OwnerId == opponentId);
        }

        public static IEnumerable<Planet> NeutralPlanets(this StatusResult gs)
        {
            return gs.Planets.Where(i => i.OwnerId == -1);
        }
    }
}