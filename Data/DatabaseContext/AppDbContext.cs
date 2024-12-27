using Microsoft.EntityFrameworkCore;
using SwissAddressManager.Data.Models;

namespace SwissAddressManager.Data.DatabaseContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Location)
                .WithMany() // No reverse navigation property in Location
                .HasForeignKey(a => a.PostalCodeID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
        }
    }
}
