using Microsoft.EntityFrameworkCore;
using SwissAddressManager.Data.Models;

namespace SwissAddressManager.Data.DatabaseContext
{
    public class AppDbContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Location> Locations { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Location)
                .WithMany(l => l.Addresses)
                .HasForeignKey(a => a.PostalCodeId);

            base.OnModelCreating(modelBuilder);
        }

    }
}
