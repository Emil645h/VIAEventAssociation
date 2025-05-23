﻿using VIAEventAssociation.Core.Domain.Common.Bases;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.Invite.ValueObjects;

public class InviteStatus : Enumeration
{
    public static readonly InviteStatus Extended = new InviteStatus(0, "Extended");
    public static readonly InviteStatus Accepted = new InviteStatus(1, "Accepted");
    public static readonly InviteStatus Rejected = new InviteStatus(2, "Rejected");
    
    private InviteStatus() { }
    private InviteStatus(int value, string displayName) : base(value, displayName) { }

    public bool CanAccept => this == Extended;
    public bool CanReject => this == Extended;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        var other = (InviteStatus)obj;
        return Value == other.Value && DisplayName == other.DisplayName;
    }
    
    public override int GetHashCode() => base.GetHashCode();
}