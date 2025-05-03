using Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example configuration for Todo<string>
            modelBuilder.Entity<Todo<string>>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.DueDate);
                entity.Property(e => e.IsCompleted).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AdditionalData).HasConversion(
                    v => v ?? string.Empty,
                    v => v
                );
            });

            // To support other T types, configure additional concrete entities similarly.
        }
    }

}
