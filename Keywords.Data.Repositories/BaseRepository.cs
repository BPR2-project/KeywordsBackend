using Keywords.Data.Repositories.Interfaces;

namespace Keywords.Data.Repositories;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

[ExcludeFromCodeCoverage]
public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : BaseModel
{
    internal readonly KeywordsContext Context;
    internal readonly DbSet<TEntity> DbSet;

    public BaseRepository(KeywordsContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual void Delete(TEntity entityToDelete, string email)
    {
        DbSet.Attach(entityToDelete);

        entityToDelete.DestroyedAt = DateTimeOffset.UtcNow;
        entityToDelete.DestroyedBy = email;

        Context.Entry(entityToDelete).State = EntityState.Modified;
    }

    public virtual void Delete(Guid id, string email)
    {
        var entityToDelete = DbSet.FirstOrDefault(a => a.Id == id);
        if (entityToDelete == null)
            throw new KeyNotFoundException($"Entry with ID {id} doesn't exist");

        entityToDelete.DestroyedAt = DateTimeOffset.UtcNow;
        entityToDelete.DestroyedBy = email;

        Context.Entry(entityToDelete).State = EntityState.Modified;
    }

    public virtual void Delete(List<TEntity> entitiesToDelete, string email)
    {
        entitiesToDelete.ForEach(entityToDelete => Delete(entityToDelete, email));
    }

    public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "",
        bool noTracking = false)
    {
        IQueryable<TEntity> query = DbSet;

        if (filter != null) query = query.Where(filter);

        if (includeProperties != null)
            foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

        query = query.Where(a => a.DestroyedAt == null && a.DestroyedBy == null);

        if (noTracking)
            query = query.AsNoTracking();

        if (orderBy != null)
            return orderBy(query).ToList();
        return query.ToList();
    }

    public virtual TEntity? GetById(Guid id)
    {
        var query = GetQueryWithAllIncludes();

        query = query.Where(a => a.DestroyedAt == null && a.DestroyedBy == null &&
                                 a.Id == id);

        return CheckSoftDelete(query.FirstOrDefault());
    }

    public virtual void Insert(TEntity entity, string email)
    {
        if (DbSet.Any(a => a.Id == entity.Id))
            return;

        entity.CreatedAt = DateTimeOffset.UtcNow;
        entity.CreatedBy = email;

        DbSet.Add(entity);
    }

    public virtual void InsertRange(IEnumerable<TEntity> entities, string email)
    {
        var entitiesToInsert = entities.Where(a => !DbSet.Any(b => b.Id == a.Id)).ToList();
        
        var now = DateTimeOffset.UtcNow;
        
        foreach (var entity in entitiesToInsert)
        {
            entity.CreatedAt = now;
            entity.CreatedBy = email;
        }

        DbSet.AddRange(entitiesToInsert);
    }
    
    public virtual void Update(TEntity entity, string email)
    {
        DbSet.Attach(entity);

        entity.UpdatedAt = DateTimeOffset.UtcNow;
        entity.UpdatedBy = email;

        Context.Entry(entity).State = EntityState.Modified;
    }

    public virtual void Save()
    {
        Context.SaveChanges();
    }

    public virtual void SaveAndStopTracking()
    {
        Context.SaveChanges();

        foreach (var entry in Context.ChangeTracker.Entries())
            entry.State = EntityState.Detached;
    }

    public abstract IQueryable<TEntity> GetQueryWithAllIncludes();
    public abstract TEntity CheckSoftDelete(TEntity? entity);

    public List<TEntity> CheckSoftDelete(TEntity[]? entities)
    {
        if (entities == null || !entities.Any())
            return new List<TEntity>();

        for (var i = 0; i < entities.Length; i++)
            entities[i] = CheckSoftDelete(entities[i]);

        return entities.ToList();
    }
}