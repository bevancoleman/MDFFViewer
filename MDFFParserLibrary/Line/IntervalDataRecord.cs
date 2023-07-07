using MDFFParserLibrary.Field;
using MDFFParserLibrary.Models;

namespace MDFFParserLibrary.Line;

public class IntervalDataRecord
{
    public BaseNem ParseLine(string[] lineSplit)
    {
        ////300,20211011,0.22600000000000,0.22200000000000,0.22800000000000,0.22900000000000,0.23300000000000,0.24400000000000,0.22800000000000,0.21500000000000,0.24100000000000,0.23800000000000,0.20800000000000,0.22800000000000,0.17500000000000,0.18800000000000,0.54700000000000,0.26400000000000,0.12900000000000,0.00400000000000,0.02000000000000,0.02000000000000,0.01500000000000,0.01600000000000,0.00600000000000,0.00000000000000,0.00000000000000,0.00000000000000,0.00000000000000,0.00000000000000,0.00000000000000,0.01300000000000,0.00000000000000,0.00000000000000,0.00000000000000,0.00000000000000,0.00000000000000,0.00300000000000,0.00900000000000,0.00600000000000,0.00500000000000,0.00400000000000,0.00500000000000,0.01300000000000,0.00800000000000,0.00900000000000,0.00900000000000,0.00900000000000,0.00700000000000,0.00500000000000,A,,,20211012034438,

        if (lineSplit.Length < 8) throw new ApplicationException("Invalid Interval Data Record (length)");

        if (lineSplit[0] != "300") throw new ApplicationException("Invalid Interval Data Record (id)");

        // RecordIndicator = lineSplit[0]
        var intervalDate = Dates.ParseDate(lineSplit[1]);

        var IntervalValue = Enumerable.Range(2, lineSplit.Length - 5 - 2).Select(x => decimal.Parse(lineSplit[x]))
            .ToArray();


        var QualityMethod = lineSplit[lineSplit.Length - 5];
        var ReasonCode = Ints.ParseInt(lineSplit[lineSplit.Length - 4]);
        var ReasonDescription = lineSplit[lineSplit.Length - 3];
        var UpdateDateTime = Dates.ParseDateTime(lineSplit[lineSplit.Length - 2]);
        var MSATSLoadDateTime = Dates.ParseDateTime(lineSplit[lineSplit.Length - 1]);

        return new IntervalDataRecord300(intervalDate.Value, IntervalValue, QualityMethod, ReasonCode,
            ReasonDescription,
            UpdateDateTime.Value, MSATSLoadDateTime);
    }
}