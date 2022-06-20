namespace Challenge.Server.Data
{
    using System;
    using System.Collections.Generic;

    public class Lobby
    {
        public Lobby()
        {
            Games = new HashSet<Game>();
            Players = new HashSet<Player>();
        }

        public string Id { get; set; }

        public string OwnerId { get; set; }

        public Player Owner { get; set; }

        /// <summary>
        /// Lobby type that will be played
        /// </summary>
        public string LobbyType { get; set; }

        /// <summary>
        /// Game type that will be played
        /// </summary>
        public string GameType { get; set; }

        /// <summary>
        /// Max players that the lobby can contain
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Random seed for the lobby (for pairings etc.)
        /// </summary>
        public int Seed { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? DeletedTime { get; set; }

        public HashSet<Game> Games { get; set; }

        public HashSet<Player> Players { get; set; }
    }
}