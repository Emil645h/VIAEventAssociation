using VIAEventAssociation.Core.Domain.Contracts;

namespace VIAEventAssociation.Core.Domain.Common.Values;

public class ActualCurrentTime : ICurrentTime
{
    public DateTime GetCurrentTime()
    {
        return DateTime.Now;
    }
}