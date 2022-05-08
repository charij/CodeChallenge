namespace PlanetWars.Common.Game.Comm
{
    using System.Collections.Generic;

    public class CommandResult
    {
        public bool Success => Errors.Count > 0;

        public string Message { get; set; }

        public List<Command> Errors { get; set; }
    }
}