using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models;

namespace kickerapi
{
    public class KickerContext : DbContext
    {
        public KickerContext(DbContextOptions<KickerContext> options)
            : base(options)
        {

        }

        // For testing
        public KickerContext()
        {

        }

        // Need to be virtual for mocking
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Match> Matches { get; set; }
        public virtual DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>().HasOne(x => x.Team1);
            modelBuilder.Entity<Match>().HasOne(x => x.Team2);

            modelBuilder.Entity<Team>().HasOne(x => x.Attacker);
            modelBuilder.Entity<Team>().HasOne(x => x.Defender);
        }
    }
}
