namespace PlanetWars.Server.Data
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;

    public class Player : IdentityUser
    {
        public Player()
        {
            GamePlayers = new HashSet<GamePlayer>();
            CommandSets = new HashSet<CommandSet>();
            LobbiesOwned = new HashSet<Lobby>();
        }

        public string LobbyId { get; set; }

        public Lobby Lobby { get; set; }

        public HashSet<GamePlayer> GamePlayers { get; set; }

        public HashSet<CommandSet> CommandSets { get; set; }

        public HashSet<Lobby> LobbiesOwned { get; set; }
    }
}