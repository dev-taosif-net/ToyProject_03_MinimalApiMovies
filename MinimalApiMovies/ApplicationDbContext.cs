using Microsoft.EntityFrameworkCore;
using MinimalApiMovies.Entities;

namespace MinimalApiMovies;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Genre> Genres { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Genre>(entity =>
        {
            // entity.ToTable(schema: "Genre"); // Only schema is specified
            entity.ToTable("Genres", schema: "Genre");
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);
        });
    }
}