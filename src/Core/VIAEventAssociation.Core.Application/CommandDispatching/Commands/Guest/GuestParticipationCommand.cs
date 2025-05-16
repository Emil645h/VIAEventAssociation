using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

public class GuestParticipationCommand
{
    public EventId EventId { get; }
    public GuestId GuestId { get; }

    private GuestParticipationCommand(EventId eventId, GuestId guestId)
    {
        EventId = eventId;
        GuestId = guestId;
    }

    public static Result<GuestParticipationCommand> Create(string eventId, string guestId)
    {
        var eid = EventId.FromString(eventId);
        var gid = GuestId.FromString(guestId);

        return Result<None>.Combine(
            eid.Map(_ => new None()),
            gid.Map(_ => new None())
        ).WithPayloadIfSuccess(() =>
            new GuestParticipationCommand(eid.Value, gid.Value));
    }
}