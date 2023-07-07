namespace MDFFParserLibrary.Models;

public record IntervalEventRecord400(int StartInterval, int EndInterval, string QualityMethod, int ReasonCode,
        string ReasonDescription)
    : BaseNem(RecordIndicator)
{
    public const int RecordIndicator = 400;
}