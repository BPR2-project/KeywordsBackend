using Microsoft.EntityFrameworkCore;

namespace Keywords.Data;

public class KeywordsContext: DbContext
{
    public KeywordsContext(DbContextOptions options): base(options)
    {
        Database.Migrate();
    }
    
    public DbSet<KeywordEntity> KeywordEntities { get; set; }
    // public DbSet<VideoEntity> VideoEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder = KeywordEntity.OnModelBuilder(modelBuilder);
        // modelBuilder = VideoEntity.OnModelBuilder(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
    
}