﻿namespace VIAEventAssociation.Core.Domain.Common.Bases;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    protected AggregateRoot(TId id) : base(id) { }
    protected AggregateRoot() : base() { }
}