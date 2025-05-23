using VIAEventAssociation.Core.Domain.Contracts;

namespace VIAEventAssociation.Infrastructure.SqliteDataRead.Common;

public class ActualCurrentTime : ICurrentTime
{
    public DateTime GetCurrentTime()
    {
        return DateTime.Now;
    }
}