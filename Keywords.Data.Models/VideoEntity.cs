// using Microsoft.EntityFrameworkCore;
//
// namespace Keywords.Data;
//
// public class VideoEntity: BaseModel 
// {
//     public string? Link { get; set; }
//     public string? Title { get; set; }
//     
//     public ICollection<KeywordEntity> KeywordEntities { get; set; }
//
//     public static ModelBuilder OnModelBuilder(ModelBuilder modelBuilder)
//     {
//         modelBuilder.Entity<VideoEntity>()
//             .HasMany(a => a.KeywordEntities);
//
//         return modelBuilder;
//     }
// }