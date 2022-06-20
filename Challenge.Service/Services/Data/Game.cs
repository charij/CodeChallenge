namespace Challenge.Server.Data
{
    using System;
    using System.Collections.Generic;

    public class Game
    {
        public Game() 
        {
            GamePlayers = new HashSet<GamePlayer>();
        }

        public string Id { get; set; }
        
        public string LobbyId { get; set; }

        public Lobby Lobby { get; set; }

        public int Seed { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? DeletedTime { get; set; }

        public HashSet<GamePlayer> GamePlayers { get; set; }
    }
}