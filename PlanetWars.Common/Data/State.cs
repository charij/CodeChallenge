namespace PlanetWars.Common.Data
{
    using System.Collections.Generic;

    public class State
    {
        public int TurnNumber { get; set; }

        public List<Planet> Planets { get; } = new();

        public List<Fleet> Fleets { get; } = new();

        public bool IsGameOver { get; set; }
    }
}