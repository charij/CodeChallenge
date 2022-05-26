namespace PlanetWars.Server.Services
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using PlanetWars.Server.Data;

    public class GameDbContext : IdentityDbContext<Player, IdentityRole, string>
    {
        public GameDbContext(DbContextOptions<GameDbContext> options)
            : base(options)
        { }

        public DbSet<Command> Commands { get; set; }
        public DbSet<CommandSet> CommandSets { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GamePlayer> GamePlayers { get; set; }
        public DbSet<Lobby> Lobbies { get; set; }
        public DbSet<Player> Players { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Command>(entity => 
            {
                entity.HasKey(i => i.Id);
                entity.HasOne(i => i.CommandSet)
                    .WithMany(i => i.Commands)
                    .HasForeignKey(i => i.CommandSetId);
            });

            builder.Entity<CommandSet>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.HasOne(i => i.Game)
                    .WithMany(i => i.CommandSets)
                    .HasForeignKey(i => i.GameId);
                entity.HasOne(i => i.Player)
                    .WithMany(i => i.CommandSets)
                    .HasForeignKey(i => i.PlayerId);
            });

            builder.Entity<Game>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.HasOne(i => i.Lobby)
                    .WithMany(i => i.Games)
                    .HasForeignKey(i => i.LobbyId);
            });

            builder.Entity<GamePlayer>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.HasOne(i => i.Game)
                    .WithMany(i => i.GamePlayers)
                    .HasForeignKey(i => i.GameId);
                entity.HasOne(i => i.Player)
                    .WithMany(i => i.GamePlayers)
                    .HasForeignKey(i => i.PlayerId);
            });

            builder.Entity<Lobby>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.HasOne(i => i.Owner)
                    .WithMany(i => i.LobbiesOwned)
                    .HasForeignKey(i => i.OwnerId);
            });

            builder.Entity<Player>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.HasOne(i => i.Lobby)
                    .WithMany(i => i.Players)
                    .HasForeignKey(i => i.LobbyId);
            });

            base.OnModelCreating(builder);
        }
    }
}