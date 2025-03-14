﻿using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;

public class EventDescription
{
    internal string Value { get; }
    
    internal EventDescription(string input) =>
        Value = input;
    
    public static Result<EventDescription> Create(string eventDescription)
    {
        return new EventDescription(eventDescription);
    }
}