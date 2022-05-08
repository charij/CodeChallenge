namespace PlanetWars.Common.Game.Data
{
    using System.Collections.Generic;

    public class State
    {
        public Settings Settings { get; set; }

        public List<Planet> Planets { get; } = new();

        public List<Fleet> Fleets { get; } = new();

        public List<Player> Players { get; set; } = new();

        public int CurrentTurn { get; set; }

        public bool IsGameOver { get; set; }        
    }
}