using CodeChavez.Common.Extensions;
using CodeChavez.Repository.Pattern.Extensions;
using CodeChavez.Repository.Pattern.Interfaces;
using CodeChavez.M3diator.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CodeChavez.Repository.Pattern.Postgres;

public class Repository<TEntity>(DbContext context) : IRepository<TEntity> where TEntity : DomainEntity
{
    private readonly DbContext _dbcontext;
    private readonly IM3diator _mediator;
    private readonly IQueryableCacheService _cache;

    public Repository(
        DbContext context, 
        IQueryableCacheService cache,
        IM3diator mediator) : this(context)
    {
        _dbcontext = context;
        _mediator = mediator;
        _cache = cache;
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _ = await _dbcontext.Set<TEntity>().AddAsync(entity, cancellationToken);
        _ = await _dbcontext.SaveChangesAsync(cancellationToken);

        if (_mediator.IsNotNull())
            await _mediator.DispatchDomainEntityEventsAsync(entity);

        return entity;
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbcontext.Set<TEntity>()
            .AsNoTracking()
            .AnyAsync(predicate, cancellationToken);
    }

    public async Task DeleteAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
    {
        var found = await _dbcontext.Set<TEntity>()
            .FindAsync(id, cancellationToken) ?? throw new InvalidOperationException("Entity not found");
        _ = _dbcontext.Set<TEntity>().Remove(found);
        _ = await _dbcontext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate, 
        bool readOnly = true, 
        bool cache = true, 
        CancellationToken cancellationToken = default)
    {
        if (readOnly && cache)
            return await _dbcontext.Set<TEntity>()
                .AsNoTracking()
                .Where(predicate)
                .FromCacheAsync(_cache,cancellationToken);
        else if (readOnly && cache == false)
            return await _dbcontext.Set<TEntity>()
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        else
            return await _dbcontext.Set<TEntity>()
                .Where(predicate)
                .FromCacheAsync(_cache,cancellationToken);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool readOnly = true, bool cache = true, CancellationToken cancellationToken = default)
    {
        if (readOnly && cache)
            return await _dbcontext.Set<TEntity>().AsNoTracking()
                .FromCacheSingleOrDefaultAsync(predicate, _cache, cancellationToken);
        else if (readOnly && cache == false)
            return await _dbcontext.Set<TEntity>().AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken: cancellationToken);
        else
            return await _dbcontext.Set<TEntity>()
                .FirstOrDefaultAsync(predicate, cancellationToken: cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _ = _dbcontext.Set<TEntity>().Update(entity);
        _ = await _dbcontext.SaveChangesAsync(cancellationToken);

        if (_mediator.IsNotNull())
            await _mediator.DispatchDomainEntityEventsAsync(entity);

        return entity;
    }
}
