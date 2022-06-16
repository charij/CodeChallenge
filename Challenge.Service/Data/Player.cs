namespace PlanetWars.Server.Data
{
    using System.Collections.Generic;

    public class Player
    {
        public Player()
        {
            GamePlayers = new HashSet<GamePlayer>();
            Commands = new HashSet<Command>();
            LobbiesOwned = new HashSet<Lobby>();
        }

        public string Id { get; set; }

        public string LobbyId { get; set; }

        public Lobby Lobby { get; set; }

        public HashSet<GamePlayer> GamePlayers { get; set; }

        public HashSet<Command> Commands { get; set; }

        public HashSet<Lobby> LobbiesOwned { get; set; }
    }
}