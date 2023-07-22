namespace MDFFParserLibrary.Models.Graph;

public class BasicGraph
{
    public BasicGraph()
    {
        DataSeries = new Dictionary<string, SeriesEnergy>();
        ValueSeries = new Dictionary<string, SeriesEnergy>();
    }

    /// <summary>
    /// Axis labels (Date Times).
    /// </summary>
    public AxisDateTime Axis { get; set; }
    
    /// <summary>
    /// Energy Usage Series
    /// </summary>
    public Dictionary<string, SeriesEnergy> DataSeries { get; set; }

    /// <summary>
    /// Energy Cost Series
    /// </summary>
    public Dictionary<string, SeriesEnergy> ValueSeries { get; set; }
}