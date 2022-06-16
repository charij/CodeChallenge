namespace PlanetWars.Server.Data
{
    public class GamePlayer
    {
        public string Id { get; set; }

        public string GameId { get; set; }

        public Game Game { get; set; }

        public string PlayerId { get; set; }

        public Player Player { get; set; }
    }
}