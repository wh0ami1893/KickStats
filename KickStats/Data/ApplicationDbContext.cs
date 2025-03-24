using Microsoft.EntityFrameworkCore;
using KickStats.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace KickStats.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {

        public DbSet<Elo> Elos { get; set; }
        public DbSet<EloHistory> EloHistories { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<PlayTable> PlayTables { get; set; }
        public DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PlayTable>(entity =>
            {
                entity.HasKey(pt => pt.Id);
                entity.Property(pt => pt.Name).IsRequired();

                entity.HasMany(pt => pt.Matches)
                    .WithOne()
                    .HasForeignKey("PlayTableId")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pt => pt.CurrentMatch)
                    .WithOne()
                    .HasForeignKey<PlayTable>(pt => pt.Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Match>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.MatchDate).IsRequired();
                entity.Property(m => m.EndTime).IsRequired();

                entity.HasOne(m => m.PlayTable)
                    .WithMany(pt => pt.Matches)
                    .HasForeignKey(m => m.PlayTableId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Team1)
                    .WithMany()
                    .HasForeignKey(m => m.Team1Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Team2)
                    .WithMany()
                    .HasForeignKey(m => m.Team2Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasMany(u => u.Teams)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.CurrentElo)
                    .WithOne(e => e.Player)
                    .HasForeignKey<Elo>(e => e.Id)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Elo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Score).IsRequired();
            });

            builder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.HasMany(t => t.Players)
                    .WithMany()
                    .UsingEntity<Dictionary<string, object>>(
                        "TeamPlayers",
                        j => j.HasOne<ApplicationUser>()
                            .WithMany()
                            .HasForeignKey("PlayerId")
                            .OnDelete(DeleteBehavior.Restrict),
                        j => j.HasOne<Team>()
                            .WithMany()
                            .HasForeignKey("TeamId")
                            .OnDelete(DeleteBehavior.Cascade)
                    );

                entity.HasMany(t => t.Matches)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });


        }
    }
}
