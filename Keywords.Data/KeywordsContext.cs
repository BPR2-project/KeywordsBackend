using Microsoft.EntityFrameworkCore;

namespace Keywords.Data;

public class KeywordsContext: DbContext
{
    public KeywordsContext(DbContextOptions options): base(options)
    {
        Database.Migrate();
    }
    
    public DbSet<KeywordEntity> KeywordEntities { get; set; }
    public DbSet<IndexerEntity> IndexerEntities { get; set; }
}