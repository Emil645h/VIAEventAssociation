namespace VIAEventAssociation.Infrastructure.SqliteDataRead.Common;

public static class DateTimeFormatHelper
{
    public static string FormatDate(string? isoDateTime)
    {
        if (string.IsNullOrEmpty(isoDateTime)) return "";
        
        if (DateTime.TryParse(isoDateTime, out var dateTime))
        {
            return dateTime.ToString("d/M-yy"); // Format like "8/3-24"
        }
        return isoDateTime;
    }

    public static string FormatTime(string? isoDateTime)
    {
        if (string.IsNullOrEmpty(isoDateTime)) return "";
        
        if (DateTime.TryParse(isoDateTime, out var dateTime))
        {
            return dateTime.ToString("HH:mm"); // Format like "19:00"
        }
        return isoDateTime;
    }

    public static string FormatDateForProfile(string? isoDateTime)
    {
        if (string.IsNullOrEmpty(isoDateTime)) return "";
        
        if (DateTime.TryParse(isoDateTime, out var dateTime))
        {
            return dateTime.ToString("dd/MM/yy"); // Format like "08/03/24" for profile page
        }
        return isoDateTime;
    }
}