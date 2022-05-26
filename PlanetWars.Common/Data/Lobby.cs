namespace PlanetWars.Common.Data
{
    using System.Collections.Generic;

    public class Lobby
    {
        public string Id { get; set; }

        public LobbySettings Settings { get; set; }

        public List<string> Players { get; set; } = new();

        public List<Game> Games { get; set; } = new();
    }
}