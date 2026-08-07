using GradFix_app_be.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GradFix_app_be.Infrastructure
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<ReportStatus> ReportStatuses { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;
        public DbSet<ReportImage> ReportImages { get; set; } = null!;
        public DbSet<ReportStatusHistory> ReportStatusHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relations
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Reports)
                .WithOne(r => r.Category)
                .HasForeignKey(r => r.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.Status)
                .WithMany()
                .HasForeignKey(r => r.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reporter -> ApplicationUser (Identity) relationship: when user deleted, keep reports (set null)
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Reporter)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.ReporterId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Report>()
                .HasMany(r => r.Images)
                .WithOne()
                .HasForeignKey(i => i.ReportId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Report>()
                .HasMany(r => r.StatusHistory)
                .WithOne(h => h.Report)
                .HasForeignKey(h => h.ReportId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Urban furniture" },
                new Category { Id = 2, Name = "Public lighting" },
                new Category { Id = 3, Name = "Traffic infrastructure" },
                new Category { Id = 4, Name = "Vegetation" },
                new Category { Id = 5, Name = "Other" }
            );

            // Seed report statuses (domain entity replaces enums)
            modelBuilder.Entity<ReportStatus>().HasData(
                new ReportStatus { Id = 1, Name = "New", Order = 1 },
                new ReportStatus { Id = 2, Name = "Accepted", Order = 2 },
                new ReportStatus { Id = 3, Name = "In progress", Order = 3 },
                new ReportStatus { Id = 4, Name = "Resolved", Order = 4 },
                new ReportStatus { Id = 5, Name = "Closed", Order = 5 }
            );
        }
    }
}
