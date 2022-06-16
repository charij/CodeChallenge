namespace PlanetWars.App.Data
{
    using System;
    using System.Collections.Generic;

    public class CommandSet
    {
        public CommandSet() 
        {
            Commands = new HashSet<Command>();
        }

        public string Id { get; set; }

        public string GameId { get; set; }

        public Game Game { get; set; }

        public string PlayerId { get; set; }

        public Player Player { get; set; }

        public int Turn { get; set; }

        public DateTime Time { get; set; }

        public HashSet<Command> Commands { get; set; }
    }
}