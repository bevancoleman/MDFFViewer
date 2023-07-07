namespace MDFFParserLibrary.Models.Graph;

public class BasicGraph
{
    public BasicGraph()
    {
        DataSeries = new Dictionary<string, SeriesDecimal>();
        ValueSeries = new Dictionary<string, SeriesDecimal>();
    }

    public AxisDateTime Axis { get; set; }
    public Dictionary<string, SeriesDecimal> DataSeries { get; set; }

    public Dictionary<string, SeriesDecimal> ValueSeries { get; set; }
}