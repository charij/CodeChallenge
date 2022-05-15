namespace PlanetWars.Server.Services
{
    using System.Data.Entity;

    public class GameDbContext : DbContext
    {
        public GameDbContext()
            : base()
        { }

       // public DbSet<Game> Games { get; set; }


        public void override OnModelCreating() 
        {
        }
    }
}