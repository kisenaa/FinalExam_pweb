using Microsoft.EntityFrameworkCore;
using Server.Models.Database;

namespace Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }
    // Database Entities
    public virtual required DbSet<User> Users { get; set; }
    public virtual required DbSet<RefreshToken> RefreshTokens { get; set; }
    
    // Overrides Relationship
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.PasswordHash)
                .HasConversion(
                    v => v.ToArray(), 
                    v => new Memory<byte>(v)); 

            entity.HasIndex(e => e.Username)
                .IsUnique();

            entity.HasIndex(e => e.Email)
                .IsUnique();
        });
    }
}