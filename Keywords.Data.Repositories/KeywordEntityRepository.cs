using Keywords.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Keywords.Data.Repositories;

public class KeywordEntityRepository: BaseRepository<KeywordEntity>, IKeywordEntityRepository
{
    public KeywordEntityRepository(KeywordsContext context) : base(context)
    {
    }

    public override IQueryable<KeywordEntity> GetQueryWithAllIncludes()
    {
        var query = DbSet as IQueryable<KeywordEntity>;

        query = query
            .AsNoTracking()
            .AsSplitQuery();

        return query;
    }

    public override KeywordEntity CheckSoftDelete(KeywordEntity? entity)
    {
        if (entity == null)
            return null;

        if (entity.DestroyedAt != null || entity.DestroyedBy != null)
            return null;

        return entity;
    }
    
    public bool ExistsById(Guid id)
    {
        return DbSet.Any(a => a.Id == id
                              && a.DestroyedAt == null
                              && a.DestroyedBy == null);
    }

    public bool KeywordsVideoExistsById(Guid videoId)
    {
        return DbSet.Any(a => a.VideoId == videoId && a.DestroyedAt == null && a.DestroyedBy == null);
    }

    public (List<KeywordEntity>? keywords, int totalsize) GetAllKeywordsByVideoId(Guid videoId, int size, int page, bool? published)
    {
        var query = GetQueryWithAllIncludes();

        query = query.Where(a => a.DestroyedAt == null && a.DestroyedBy == null && a.VideoId == videoId);

        if (published != null && published == true)
        {
            query = query.Where(a => a.IsPublished == published);
        }
        
        var totalSize = query.Count();

        query = query
            .Skip(size * page)
            .Take(size)
            .OrderBy(a => a.CreatedAt);

        return (query.ToList(), totalSize);
    }
}