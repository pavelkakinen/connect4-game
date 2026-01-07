using Microsoft.EntityFrameworkCore;

namespace DAL.EF;

public class AppDbContext : DbContext
{
    public DbSet<GameState> Games { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Remove all cascade deletion
        foreach (var relationship in modelBuilder.Model
                     .GetEntityTypes()
                     .Where(e => !e.IsOwned())
                     .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
        
        // GameState configuration
        modelBuilder.Entity<GameState>(entity =>
        {
            entity.HasKey(e => e.GameId);
            
            entity.Property(e => e.GameId)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Player1Name)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.Player2Name)
                .IsRequired()
                .HasMaxLength(50);
            
            // Board как JSON строка
            entity.Property(e => e.Board)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<List<int>>>(v, (System.Text.Json.JsonSerializerOptions?)null)!
                );
        });
    }
}