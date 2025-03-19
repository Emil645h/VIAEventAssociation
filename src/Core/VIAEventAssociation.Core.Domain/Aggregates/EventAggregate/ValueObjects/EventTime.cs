using VIAEventAssociation.Core.Domain.Common.Values;
using VIAEventAssociation.Core.Domain.Contracts;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;

public class EventTime
{
    internal DateTime StartTime { get; }
    internal DateTime EndTime { get; }

    private EventTime(DateTime startTime, DateTime endTime)
        => (StartTime, EndTime) = (startTime, endTime);

    public static Result<EventTime> Create(DateTime startTime, DateTime endTime)
        => Validate(startTime, endTime, new ActualCurrentTime());

    private static Result<EventTime> Validate(DateTime startTime, DateTime endTime, ICurrentTime currentTime)
        => ResultExtensions.AssertAll(
            () => ValidateStartDateNotAfterEndDate(startTime, endTime),
            () => ValidateStartTimeNotAfterEndTimeOnSameDay(startTime, endTime),
            () => ValidateDurationIsAtLeastOneHour(startTime, endTime),
            () => ValidateDurationIsNotLongerThanTenHours(startTime, endTime),
            () => ValidateStartTimeNotBefore08(startTime),
            () => ValidateEndTimeNotAfter01(startTime, endTime),
            () => ValidateDoesNotSpanInvalidTimeSlot(startTime, endTime),
            () => ValidateStartTimeIsInFuture(startTime, currentTime)
            ).WithPayloadIfSuccess(() => new EventTime(startTime, endTime));

    private static Result<None> ValidateStartDateNotAfterEndDate(DateTime startTime, DateTime endTime)
    {
        if (startTime.Date > endTime.Date)
        {
            return EventErrors.EventTime.StartDateAfterEndDate;
        }
        return new None();
    }
    
    private static Result<None> ValidateStartTimeNotAfterEndTimeOnSameDay(DateTime startTime, DateTime endTime)
    {
        if (startTime.Date == endTime.Date && startTime.TimeOfDay > endTime.TimeOfDay)
        {
            return EventErrors.EventTime.StartTimeAfterEndTime;
        }
        return new None();
    }
    
    private static Result<None> ValidateDurationIsAtLeastOneHour(DateTime startTime, DateTime endTime)
    {
        TimeSpan duration = endTime - startTime;
        if (duration.TotalHours < 1)
        {
            return EventErrors.EventTime.DurationTooShort;
        }
        return new None();
    }
    
    private static Result<None> ValidateDurationIsNotLongerThanTenHours(DateTime startTime, DateTime endTime)
    {
        TimeSpan duration = endTime - startTime;
        if (duration.TotalHours > 10)
        {
            return EventErrors.EventTime.DurationTooLong;
        }
        return new None();
    }
    
    private static Result<None> ValidateStartTimeNotBefore08(DateTime startTime)
    {
        TimeOnly startTimeOnly = TimeOnly.FromDateTime(startTime);
        if (startTimeOnly < new TimeOnly(8, 0))
        {
            return EventErrors.EventTime.StartTimeTooEarly;
        }
        return new None();
    }
    
    private static Result<None> ValidateEndTimeNotAfter01(DateTime startTime, DateTime endTime)
    {
        TimeOnly endTimeOnly = TimeOnly.FromDateTime(endTime);
        bool isNextDay = endTime.Date > startTime.Date;
        
        if (isNextDay && endTimeOnly > new TimeOnly(1, 0))
        {
            return EventErrors.EventTime.EndTimeTooLate;
        }
        return new None();
    }
    
    private static Result<None> ValidateDoesNotSpanInvalidTimeSlot(DateTime startTime, DateTime endTime)
    {
        TimeOnly startTimeOnly = TimeOnly.FromDateTime(startTime);
        TimeOnly endTimeOnly = TimeOnly.FromDateTime(endTime);
        bool isNextDay = endTime.Date > startTime.Date;
        
        // Case 1: Event starts before or at 01:00 and ends after or at 08:00 on the same day
        if (startTimeOnly <= new TimeOnly(1, 0) && endTimeOnly >= new TimeOnly(8, 0) && startTime.Date == endTime.Date)
        {
            return EventErrors.EventTime.InvalidTimeSpan;
        }
        
        // Case 2: Event starts on one day and ends on the next, spans the invalid period
        if (isNextDay && (endTimeOnly >= new TimeOnly(1, 0, 1) || startTimeOnly <= new TimeOnly(1, 0)))
        {
            if (endTimeOnly >= new TimeOnly(8, 0))
            {
                return EventErrors.EventTime.InvalidTimeSpan;
            }
        }
        return new None();
    }
    
    private static Result<None> ValidateStartTimeIsInFuture(DateTime startTime, ICurrentTime currentTime)
    {
        if (startTime < currentTime.GetCurrentTime())
        {
            return EventErrors.EventTime.StartTimeInPast;
        }
        return new None();
    }
    
}