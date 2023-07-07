namespace MDFFParserLibrary.Field;

public static class Ints
{
    public static int? ParseInt(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return null;

        int result;
        if (int.TryParse(s, out result))
            return result;
        return null;
    }
}