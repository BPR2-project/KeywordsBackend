using Microsoft.EntityFrameworkCore;

namespace Keywords.Data;

public class KeywordEntity: BaseModel
{
    public string Content { get; set; }
    public bool IsPublished { get; set; }
    public Guid VideoId { get; set; }

    // public static ModelBuilder OnModelBuilder(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<KeywordEntity>()
    //         .HasOne(a => a.Video);
    //
    //     return modelBuilder;
    // }
}