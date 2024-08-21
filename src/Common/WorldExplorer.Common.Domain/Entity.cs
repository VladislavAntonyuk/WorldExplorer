namespace WorldExplorer.Common.Domain;

public abstract class Entity
{
    private readonly List<IDomainEvent> domainEvents = [];

    protected Entity()
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.ToList();

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }

    protected void Raise(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}
