namespace MDFFParserLibrary.Models.Graph;

public class AxisDateTime
{
    public AxisDateTime()
    {
        Values = new Dictionary<string, DateTime>();
    }

    public string Name { get; set; }
    public Dictionary<string, DateTime> Values { get; set; }
}