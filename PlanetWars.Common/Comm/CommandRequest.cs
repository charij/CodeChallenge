namespace PlanetWars.Common.Comm
{
    using System;

    public class CommandRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid SourcePlanetId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Guid TargetPlanetId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ShipCount { get; set; }
    }
}