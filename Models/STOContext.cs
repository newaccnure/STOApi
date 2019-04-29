using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace STOApi.Models
{
    public class STOContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Tournament> Tournaments { set; get; }
        public DbSet<Sport> Sports { set; get; }
        public DbSet<EventFormat> EventFormats { set; get; }


        public STOContext(DbContextOptions<STOContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
