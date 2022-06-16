namespace PlanetWars.App.Data
{
    public class Command
    {
        public string Id { get; set; }

        public string CommandSetId { get; set; }

        public CommandSet CommandSet { get; set; }

        public string SourcePlanet { get; set; }

        public string TargetPlanet { get; set; }

        public int ShipCount { get; set; }
    }
}