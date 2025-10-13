using Microsoft.EntityFrameworkCore;
using AccessManagerWeb.Core.Models;

namespace AccessManagerWeb.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ResourceRequest> ResourceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            modelBuilder.Entity<ResourceRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RequestorUsername).IsRequired();
                entity.Property(e => e.ResourceName).IsRequired();
                entity.Property(e => e.Status).IsRequired();
            });
        }
    }
}