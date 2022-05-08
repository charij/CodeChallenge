namespace PlanetWars.Common.Game.Data
{
    public class Settings
    {
        public int Seed;

        public long StartDelay = 2000;
        public long PlayerTurnLength = 800;
        public long ServerTurnLength = 200;
        
        public int TurnLimit = 200;

        public int MinRadius = 20;
        public int MaxRadius = 1000;
        
        public int MinPlanetCount = 5;
        public int MaxPlanetCount = 20;

        public int MinPlanetGrowthRate = 1;
        public int MaxPlanetGrowthRate = 10;

        public int MinPlanetShips = 0;
        public int MaxPlanetShips = 100;
    }
}