namespace PlanetWars.App.Data
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Game
    {
        public Game() 
        {
            CommandSets = new HashSet<CommandSet>();
            GamePlayers = new HashSet<GamePlayer>();
        }

        public string Id { get; set; }
        
        public string LobbyId { get; set; }

        public Lobby Lobby { get; set; }

        public bool IsActive { get; set; }

        public int Seed { get; set; }

        public HashSet<CommandSet> CommandSets { get; set; }

        public HashSet<GamePlayer> GamePlayers { get; set; }
    }
}