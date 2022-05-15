namespace PlanetWars.Common.Data
{
    using System;

    public class Planet
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public Point Position { get; set; }

        public int GrowthRate { get; set; }

        public int ShipCount { get; set; }

        public Planet() 
        {
        }

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