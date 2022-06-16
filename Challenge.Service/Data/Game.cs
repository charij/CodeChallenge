namespace PlanetWars.Server.Data
{
    using System.Collections.Generic;

    public class Game
    {
        public Game() 
        {
            Commands = new HashSet<Command>();
            GamePlayers = new HashSet<GamePlayer>();
        }

        public string Id { get; set; }
        
        public string LobbyId { get; set; }

        public Lobby Lobby { get; set; }

        public bool IsActive { get; set; }

        public int Seed { get; set; }

        public HashSet<Command> Commands { get; set; }

        public HashSet<GamePlayer> GamePlayers { get; set; }
    }
}