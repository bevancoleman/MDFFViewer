using MDFFParserLibrary.Field;
using MDFFParserLibrary.Models;

namespace MDFFParserLibrary.Line;

public class NmiDataDetails
{
    public BaseNem ParseLine(string[] lineSplit)
    {
        //200,2001076293,E1B1E2,E1,E1,,123456789,KWH,30,

        if (lineSplit.Length != 10) throw new ApplicationException("Invalid Nmi Data Details Record (length)");

        if (lineSplit[0] != "200") throw new ApplicationException("Invalid Nmi Data Details Record (id)");

        // RecordIndicator = lineSplit[0]
        var nmi = lineSplit[1];
        var nmiconfiguration = lineSplit[2];
        var registerId = lineSplit[3];
        var nmiSuffix = lineSplit[4];
        var mdmDSId = lineSplit[5];
        var meterSerialNumber = lineSplit[6];
        var uom = UnitOfMeasure.Parse(lineSplit[7]);
        var intervalLength = Ints.ParseInt(lineSplit[8]).Value;
        var nextRead = Dates.ParseDate(lineSplit[9]);

        return new NmiDataDetails200(nmi, nmiconfiguration, registerId, nmiSuffix, mdmDSId, meterSerialNumber, uom,
            intervalLength, nextRead);
    }
}