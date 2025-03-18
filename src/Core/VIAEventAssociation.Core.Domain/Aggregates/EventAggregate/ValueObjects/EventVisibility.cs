using VIAEventAssociation.Core.Domain.Common.Bases;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;

public class EventVisibility : Enumeration
{
    public static readonly EventVisibility Private = new EventVisibility(0, "Private");
    public static readonly EventVisibility Public = new EventVisibility(1, "Public");
    
    private EventVisibility() { }
    private EventVisibility(int value, string displayName) : base(value, displayName) { }
}