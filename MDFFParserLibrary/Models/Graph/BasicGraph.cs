namespace MDFFParserLibrary.Models.Graph;

public class BasicGraph
{
    public BasicGraph()
    {
        DataSeries = new Dictionary<string, SeriesDecimal>();
        ValueSeries = new Dictionary<string, SeriesDecimal>();
    }

    /// <summary>
    /// Axis labels (Date Times).
    /// </summary>
    public AxisDateTime Axis { get; set; }
    
    /// <summary>
    /// Energy Usage Series
    /// </summary>
    public Dictionary<string, SeriesDecimal> DataSeries { get; set; }

    /// <summary>
    /// Energy Cost Series
    /// </summary>
    public Dictionary<string, SeriesDecimal> ValueSeries { get; set; }
}