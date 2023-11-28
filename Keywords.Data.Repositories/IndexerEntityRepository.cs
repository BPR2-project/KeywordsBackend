using Keywords.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Keywords.Data.Repositories;


public class IndexerEntityRepository : BaseRepository<IndexerEntity>, IIndexerEntityRepository
{
    public IndexerEntityRepository(KeywordsContext context) : base(context)
    {
    }

    public override IQueryable<IndexerEntity> GetQueryWithAllIncludes()
    {
        var query = DbSet as IQueryable<IndexerEntity>;

        query = query
            .AsNoTracking()
            .AsSplitQuery();

        return query;
    }

    public override IndexerEntity CheckSoftDelete(IndexerEntity? entity)
    {
        if (entity == null)
            return null;

        if (entity.DestroyedAt != null || entity.DestroyedBy != null)
            return null;

        return entity;
    }
}