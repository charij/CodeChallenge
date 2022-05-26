namespace PlanetWars.Common.Data
{
    using System.Collections.Generic;

    public class Game
    {
        public string Id { get; set; }

        public GameSettings Settings { get; set; }

        public List<string> Players { get; set; } = new();

        public List<State> History { get; set; }
    }
}