namespace PlanetWars.Common.Data
{
    using System;
    using System.Collections.Generic;

    public class GameDetails
    {
        public Guid Id { get; set; }

        public Settings Settings { get; set; }

        public List<Player> Players { get; set; } = new();

        public List<State> History { get; set; }

        public bool IsGameOver { get; set; }
    }
}