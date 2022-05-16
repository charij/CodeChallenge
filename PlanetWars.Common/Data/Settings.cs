namespace PlanetWars.Common.Data
{
    using System;

    public class Settings
    {
        /// <summary>
        /// 
        /// </summary>
        public int Seed { get; set; } = (int)DateTime.Now.Ticks;

        /// <summary>
        /// 
        /// </summary>
        public long StartDelay { get; set; } = 2000;

        /// <summary>
        /// 
        /// </summary>
        public long PlayerTurnLength { get; set; } = 800;

        /// <summary>
        /// 
        /// </summary>
        public long ServerTurnLength { get; set; } = 200;

        /// <summary>
        /// 
        /// </summary>
        public int TurnLimit { get; set; } = 200;

        /// <summary>
        /// 
        /// </summary>
        public int MinRadius { get; set; } = 20;

        /// <summary>
        /// 
        /// </summary>
        public int MaxRadius { get; set; } = 1000;

        /// <summary>
        /// 
        /// </summary>
        public int MinPlanetCount { get; set; } = 5;

        /// <summary>
        /// 
        /// </summary>
        public int MaxPlanetCount { get; set; } = 20;

        /// <summary>
        /// 
        /// </summary>
        public int MinPlanetGrowthRate { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        public int MaxPlanetGrowthRate { get; set; } = 10;

        /// <summary>
        /// 
        /// </summary>
        public int MinPlanetShips { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int MaxPlanetShips { get; set; } = 100;
    }
}