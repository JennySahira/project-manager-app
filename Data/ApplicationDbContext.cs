using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagerApp.Models;

namespace ProjectManagerApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }

        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Fix för decimal Budget. Skapad med hjälp av ChatGpt.
            builder.Entity<Project>()
                .Property(p => p.Budget)
                .HasColumnType("decimal(18,2)");
        }
    }
}
