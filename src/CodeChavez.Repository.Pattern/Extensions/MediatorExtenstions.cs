using CodeChavez.M3diator.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeChavez.Repository.Pattern.Extensions;

public static class MediatorExtenstions
{
    public static async Task DispatchDomainEventsAsync(this IM3diator mediator, DbContext dbContext)
    {
        var domainEntities = dbContext.ChangeTracker
            .Entries<DomainEntity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Count != 0)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents!)
            .ToList();

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);

        domainEntities
            .ForEach(entity => entity.Entity.ClearDomainEvents());
    }

    public static async Task DispatchDomainEntityEventsAsync<TEntity>(this IM3diator mediator, TEntity entity) where TEntity : DomainEntity
    {
        var domainEvents = entity.DomainEvents;
        if (domainEvents != null)
            foreach (var @event in domainEvents)
                await mediator.Publish(@event);

        entity.ClearDomainEvents();
    }

    public static async Task DispatchDomainDocumentEventsAsync<TDocument>(this IM3diator mediator, TDocument entity) where TDocument : DomainDocument
    {
        var domainEvents = entity.DomainEvents;
        if (domainEvents != null)
            foreach (var @event in domainEvents)
                await mediator.Publish(@event);

        entity.ClearDomainEvents();
    }
}
