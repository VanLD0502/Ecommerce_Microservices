using BuildingBlocks.Shared.Domains.Interfaces;
using BuildingBlocks.Shared.Events;

namespace BuildingBlocks.Shared.Domains;

public abstract class AggregateRoot<TId> : EntityBase<TId>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Check(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new DomainException(rule);
        }
    }
}