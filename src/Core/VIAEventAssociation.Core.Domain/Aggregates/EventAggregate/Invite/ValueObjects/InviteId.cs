﻿using VIAEventAssociation.Core.Domain.Aggregates.Events.GuestList;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.Events.Invite.ValueObjects;

public class InviteId
{
    public Guid Value { get; }
    
    private InviteId(Guid value) => Value = value;
    
    public static Result<InviteId> Create(Guid value) =>
        value == Guid.Empty ? InviteErrors.InviteId.IsEmpty : new InviteId(value);
}