namespace VIAEventAssociation.Core.Domain.Common.Bases;

public abstract class Entity<TId>
{
    public TId Id { get; protected set; }

    protected Entity(TId id)
    {
        Id = id;
    }
    
    protected Entity() { }
}