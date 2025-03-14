using VIAEventAssociation.Core.Domain.Common.Bases;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.ValueObjects;

public class EventStatus : Enumeration
{
    public static readonly EventStatus Draft = new EventStatus(0, "Draft");
    public static readonly EventStatus Ready = new EventStatus(1, "Ready");
    public static readonly EventStatus Active = new EventStatus(2, "Active");
    public static readonly EventStatus Cancelled = new EventStatus(3, "Cancelled");
    
    private EventStatus() {}
    private EventStatus(int value, string displayName) : base(value, displayName) { }
}