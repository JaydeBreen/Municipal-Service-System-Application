using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MunicipalServices.Models;
using System.Diagnostics.CodeAnalysis;

namespace MunicipalServices.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        [NotNull]
        public DbSet<ServiceRequest> ServiceRequests { get; set; } = null!;

        [NotNull]
        public DbSet<Event> Events { get; set; } = null!;


        // The database relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(r => r.User)
                .WithMany(u => u.ServiceRequests)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
