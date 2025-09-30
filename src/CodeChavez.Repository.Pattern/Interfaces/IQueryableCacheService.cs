using System.Linq.Expressions;

namespace CodeChavez.Repository.Pattern.Interfaces
{
    public interface IQueryableCacheService
    {
        void Clear();
        Task<List<T>> FromCacheAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default);
        Task<T?> FromCacheSingleOrDefaultAsync<T>(IQueryable<T> query, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}