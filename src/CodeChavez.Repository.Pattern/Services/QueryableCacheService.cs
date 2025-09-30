using CodeChavez.Repository.Pattern.Interfaces;
using CodeChavez.Repository.Pattern.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace CodeChavez.Repository.Pattern.Services;

public class QueryableCacheService : IQueryableCacheService
{
    private readonly IMemoryCache _cache;
    private readonly QueryableCacheSettings _settings;

    public QueryableCacheService(IMemoryCache cache, IOptions<QueryableCacheSettings> settings)
    {
        _cache = cache;
        _settings = settings.Value;
    }

    private static string GetCacheKey(IQueryable query)
    {
        var queryString = query.ToQueryString();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(queryString));
        return Convert.ToBase64String(hash);
    }

    public async Task<List<T>> FromCacheAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default)
    {
        var hashKey = GetCacheKey(query);
        return await _cache.GetOrCreateAsync(hashKey, async entry =>
        {
            entry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(_settings.ExpirationInSeconds);
            return await query.ToListAsync(cancellationToken);
        }) ?? [];
    }

    public async Task<T?> FromCacheSingleOrDefaultAsync<T>(
       IQueryable<T> query,
       Expression<Func<T, bool>> predicate,
       CancellationToken cancellationToken = default)
    {
        var filteredQuery = query.Where(predicate);
        var key = GetCacheKey(filteredQuery);

        var result = await _cache.GetOrCreateAsync(key, cache =>
        {
            cache.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(_settings.ExpirationInSeconds);
            return filteredQuery.FirstOrDefaultAsync(cancellationToken);
        });

        return result;
    }

    public void Clear()
    {
        _cache.Dispose();
    }
}