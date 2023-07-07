namespace MDFFParserLibrary.Models;

public record Header100(string VersionHeader, DateTime DateTime, string FromParticipant,
        string ToParticipant)
    : BaseNem(RecordIndicator)
{
    public const int RecordIndicator = 100;
}