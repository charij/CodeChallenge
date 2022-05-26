namespace PlanetWars.Common.Data
{
    using System;

    public class Planet
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int GrowthRate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ShipCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Planet() 
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        public Planet(Planet template) 
        {
            Id = template.Id;
            OwnerId = template.OwnerId;
            Position = template.Position;
            GrowthRate = template.GrowthRate;
            ShipCount = template.ShipCount;
        }
    }
}