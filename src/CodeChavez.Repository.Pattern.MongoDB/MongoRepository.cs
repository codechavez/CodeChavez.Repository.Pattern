using CodeChavez.Common.Extensions;
using CodeChavez.Repository.Pattern.Extensions;
using CodeChavez.Repository.Pattern.Interfaces;
using CodeChavez.M3diator.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace CodeChavez.Repository.Pattern.MongoDB;

public class MongoRepository<TEntity> : INoSQLRepository<TEntity> where TEntity : DomainDocument
{
    private readonly DbContext _dbContext;
    private readonly IM3diator _mediator;
    private readonly IQueryableCacheService _cache;

    public MongoRepository(
        DbContext dbContext,
        IQueryableCacheService cache,
        IM3diator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
        _cache = cache;
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _ = await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        _dbContext.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        await _dbContext.SaveChangesAsync();

        if (_mediator.IsNotNull())
            await _mediator.DispatchDomainDocumentEventsAsync(entity);

        return entity;
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        bool readOnly = true, bool cache = true,
        CancellationToken cancellationToken = default)
    {
        if (readOnly && cache)
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(predicate)
                .FromCacheAsync(_cache, cancellationToken);
        else if (readOnly && cache == false)
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        else
            return await _dbContext.Set<TEntity>()
                .Where(predicate)
                .FromCacheAsync(_cache, cancellationToken);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool readOnly = true, bool cache = true, CancellationToken cancellationToken = default)
    {
        if (readOnly && cache)
            return await _dbContext.Set<TEntity>().AsNoTracking()
                .FromCacheSingleOrDefaultAsync(predicate, _cache, cancellationToken);
        else if (readOnly && cache == false)
            return await _dbContext.Set<TEntity>().AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken: cancellationToken);
        else
            return await _dbContext.Set<TEntity>()
                .FirstOrDefaultAsync(predicate, cancellationToken: cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Set<TEntity>().FindAsync(entity.Id);

        if (existing == null)
            throw new KeyNotFoundException("Not found");

        if (existing != null)
            _dbContext.Set<TEntity>().Entry(existing).State = EntityState.Detached;

        _dbContext.Set<TEntity>().Attach(entity);

        _dbContext.Update(entity);
        await _dbContext.SaveChangesAsync();

        if (_mediator.IsNotNull())
            await _mediator.DispatchDomainDocumentEventsAsync(entity);

        return entity;
    }
}
