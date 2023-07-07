namespace MDFFParserLibrary.Models;

public record B2BDetailRecord500(char TransCode, string RetServiceOrder, DateTime ReadDateTime, string IndexRead)
    : BaseNem(RecordIndicator)
{
    public const int RecordIndicator = 500;
}