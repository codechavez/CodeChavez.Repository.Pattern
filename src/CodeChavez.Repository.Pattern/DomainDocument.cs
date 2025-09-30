using MediatR;

namespace CodeChavez.Repository.Pattern;

public record class DomainDocument : Document
{
    private readonly List<INotification> _domainEvents = [];
    public IReadOnlyCollection<INotification>? DomainEvents => _domainEvents?.AsReadOnly();
    public void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);
    public void RemoveDomainEvent(INotification eventItem) => _domainEvents?.Remove(eventItem);
    public void ClearDomainEvents() => _domainEvents?.Clear();
}