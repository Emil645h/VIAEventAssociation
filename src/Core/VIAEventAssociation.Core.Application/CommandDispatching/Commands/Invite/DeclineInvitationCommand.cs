namespace VIAEventAssociation.Core.Application.CommandDispatching.Commands.Invite;

using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

public class DeclineInvitationCommand
{
    public EventId EventId { get; }
    public GuestId GuestId { get; }

    private DeclineInvitationCommand(EventId eventId, GuestId guestId)
    {
        EventId = eventId;
        GuestId = guestId;
    }

    public static Result<DeclineInvitationCommand> Create(string eventId, string guestId)
    {
        var eid = EventId.FromString(eventId);
        var gid = GuestId.FromString(guestId);

        return Result<None>.Combine(
            eid.Map(_ => new None()),
            gid.Map(_ => new None())
        ).WithPayloadIfSuccess(() =>
            new DeclineInvitationCommand(eid.Value, gid.Value));
    }
}
