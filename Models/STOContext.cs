using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using STOApi.Entities;

namespace STOApi.Models
{
    public class STOContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Tournament> Tournaments { set; get; }
        public DbSet<Schedule> Schedules { set; get; }
        public DbSet<Game> Games { set; get; }
        public DbSet<Sport> Sports { set; get; }
        public DbSet<EventFormat> EventFormats { set; get; }
        public DbSet<UserTournament> UserTournament {set;get;}
        public STOContext(DbContextOptions<STOContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().OwnsOne(u => u.GameSchedule);
            modelBuilder.Entity<Game>().OwnsOne(u => u.Score);
            modelBuilder.Entity<Schedule>().OwnsOne(u => u.TournamentSchedule);


            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Schedule)
                .WithOne(s => s.Tournament)
                .HasForeignKey<Schedule>(s => s.TournamentId);

            modelBuilder.Entity<UserTournament>()
                .HasKey(t => new { t.TournamentId, t.UserId });
            modelBuilder.Entity<UserTournament>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTournaments)
                .HasForeignKey(ut => ut.UserId);
            modelBuilder.Entity<UserTournament>()
                .HasOne(ut => ut.Tournament)
                .WithMany(u => u.UserTournaments)
                .HasForeignKey(ut => ut.TournamentId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Schedule)
                .WithMany(s => s.Games)
                .OnDelete(DeleteBehavior.Cascade);


            base.OnModelCreating(modelBuilder);
        }
    }
}
