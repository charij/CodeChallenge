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

        /// <summary>
        /// Unique lobby id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Lobby owner Id
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Lobby owner
        /// </summary>
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

        /// <summary>
        /// Time when the lobby was created
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Time when the lobby started
        /// </summary>
        public DateTime? StartedTime { get; set; }

        /// <summary>
        /// Time when the lobby was completed
        /// </summary>
        public DateTime? CompletedTime { get; set; }

        /// <summary>
        /// Games generated from the lobby
        /// </summary>
        public HashSet<Game> Games { get; set; }

        /// <summary>
        /// Players participating in the lobby
        /// </summary>
        public HashSet<Player> Players { get; set; }
    }
}