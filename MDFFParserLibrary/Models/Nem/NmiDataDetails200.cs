using MDFFParserLibrary.Models.Enums;

namespace MDFFParserLibrary.Models;

public record NmiDataDetails200(string NMI, string MNIConfiguration, string RegisterId,
        string NMISuffix, string MDMDataStreamIdentifier, string MeterSerialNumber, DataUnitOfMeasure UOM,
        int IntervalLength,
        DateTime? NextScheduledReadDate)
    : BaseNem(RecordIndicator)
{
    public const int RecordIndicator = 200;
}