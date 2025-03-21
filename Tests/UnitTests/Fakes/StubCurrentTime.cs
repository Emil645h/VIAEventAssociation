using VIAEventAssociation.Core.Domain.Contracts;

namespace UnitTests.Fakes;

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