using CodeChavez.Repository.Pattern.Interfaces;
using CodeChavez.Repository.Pattern.Services;
using CodeChavez.Repository.Pattern.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace CodeChavez.Repository.Pattern.Extensions;

public static class QueryableCacheExtensions
{
    public static Task<List<T>> FromCacheAsync<T>(
        this IQueryable<T> query,
        IQueryableCacheService cache,
        CancellationToken cancellationToken = default)
    {
        return cache.FromCacheAsync(query, cancellationToken);
    }

    public static Task<T?> FromCacheSingleOrDefaultAsync<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> predicate,
        IQueryableCacheService cache,
        CancellationToken cancellationToken = default
        )
    {
        return cache.FromCacheSingleOrDefaultAsync(query, predicate, cancellationToken);
    }

    public static IServiceCollection AddQueryableCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.Configure<QueryableCacheSettings>(options => configuration.GetSection("QueryableCacheSettings").Bind(options));
        services.AddScoped<IQueryableCacheService, QueryableCacheService>();

        return services;
    }
}
