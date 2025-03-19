using VIAEventAssociation.Core.Domain.Contracts;

namespace VIAEventAssociation.Core.Domain.Common.Values;

public class StubCurrentTime : ICurrentTime
{
    public DateTime TheTime { get; set; }

    public StubCurrentTime(DateTime initialTime)
    {
        TheTime = initialTime;
    }

    public DateTime GetCurrentTime()
    {
        return TheTime;
    }
}