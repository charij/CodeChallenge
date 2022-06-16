namespace PlanetWars.Server.Data
{
    using System;

    public class Command
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GameId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PlayerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Player Player { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Commands { get; set; }
    }
}