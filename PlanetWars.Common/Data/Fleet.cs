namespace PlanetWars.Common.Game.Data
{
    using System;

    public class Fleet
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public Guid SourcePlanetId { get; set; }

        public Guid TargetPlanetId { get; set; }

        public int ShipCount { get; set; }

        public int TurnCreated { get; set; }

        public int TravelTime { get; set; }
    }
}