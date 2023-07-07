using System.Globalization;

namespace MDFFParserLibrary.Field;

public static class Dates
{
    public static DateTime? ParseDate(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return null;

        //Example: 20211003
        return DateTime.ParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture);
    }

    public static DateTime? ParseDateTime(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return null;

        //Example: 20211006022802
        return DateTime.ParseExact(s, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
    }
}