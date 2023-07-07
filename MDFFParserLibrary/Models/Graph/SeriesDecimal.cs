namespace MDFFParserLibrary.Models.Graph;

public class SeriesDecimal
{
    public SeriesDecimal(string name)
    {
        Name = name;
        Values = new List<decimal>();
    }

    public string Name { get; init; }
    public List<decimal> Values { get; }
}