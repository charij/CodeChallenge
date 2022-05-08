﻿namespace PlanetWars.Common.Game.Comm
{
    using System;

    public class Command
    {
        public Guid SourcePlanetId { get; set; }
        
        public Guid TargetPlanetId { get; set; }

        public int ShipCount { get; set; }
    }
}
