namespace MDFFParserLibrary.Models;

public record IntervalDataRecord300(DateTime IntervalDate, decimal[] IntervalValue, string QualityMethod,
    int? ReasonCode, string ReasonDescription, DateTime UpdateDateTime, DateTime? MSATSLoadDateTime) : BaseNem(
    RecordIndicator)
{
    public const int RecordIndicator = 300;
}