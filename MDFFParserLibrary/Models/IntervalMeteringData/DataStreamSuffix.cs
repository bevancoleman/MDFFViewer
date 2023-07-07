using MDFFParserLibrary.Models.Enums;

namespace MDFFParserLibrary.Models.IntervalMeteringData;

/// <summary>
///     https://www.aemo.com.au/Electricity/National-Electricity-Market-NEM/Retail-and-metering/-/media/EBA9363B984841079712B3AAD374A859.ashx
///     Section 7.
/// </summary>
public class DataStreamSuffix
{
    public DataStreamSuffix(string suffix)
    {
        if (suffix.Length != 2)
            throw new InvalidDataException("Suffix must be two chars.");

        MeterCode = suffix.Substring(0, 1);
        MeterNumber = suffix.Substring(1, 1);

        switch (MeterCode)
        {
            case "A":
                MeterType = MeterType.Ave;
                DataType = DataType.Import;
                DataUnitOfMeasure = DataUnitOfMeasure.kWh;
                break;
            case "B":
                MeterType = MeterType.Master;
                DataType = DataType.Import;
                DataUnitOfMeasure = DataUnitOfMeasure.kWh;
                break;
            case "C":
                MeterType = MeterType.Check;
                DataType = DataType.Import;
                DataUnitOfMeasure = DataUnitOfMeasure.kWh;
                break;
            case "D":
                MeterType = MeterType.Ave;
                DataType = DataType.Export;
                DataUnitOfMeasure = DataUnitOfMeasure.kWh;
                break;
            case "E":
                MeterType = MeterType.Master;
                DataType = DataType.Export;
                DataUnitOfMeasure = DataUnitOfMeasure.kWh;
                break;
            case "F":
                MeterType = MeterType.Check;
                DataType = DataType.Export;
                DataUnitOfMeasure = DataUnitOfMeasure.kWh;
                break;
            case "J":
                MeterType = MeterType.Ave;
                DataType = DataType.Import;
                DataUnitOfMeasure = DataUnitOfMeasure.kvar;
                break;
            case "K":
                MeterType = MeterType.Master;
                DataType = DataType.Import;
                DataUnitOfMeasure = DataUnitOfMeasure.kvar;
                break;
            case "L":
                MeterType = MeterType.Check;
                DataType = DataType.Import;
                DataUnitOfMeasure = DataUnitOfMeasure.kvar;
                break;
            case "P":
                MeterType = MeterType.Ave;
                DataType = DataType.Export;
                DataUnitOfMeasure = DataUnitOfMeasure.kvar;
                break;
            case "Q":
                MeterType = MeterType.Master;
                DataType = DataType.Export;
                DataUnitOfMeasure = DataUnitOfMeasure.kvar;
                break;
            case "R":
                MeterType = MeterType.Check;
                DataType = DataType.Export;
                DataUnitOfMeasure = DataUnitOfMeasure.kvar;
                break;
            case "S":
                MeterType = MeterType.Ave;
                DataType = DataType.KVAh;
                DataUnitOfMeasure = DataUnitOfMeasure.KVAh;
                break;
            case "T":
                MeterType = MeterType.Master;
                DataType = DataType.KVAh;
                DataUnitOfMeasure = DataUnitOfMeasure.KVAh;
                break;
            case "U":
                MeterType = MeterType.Check;
                DataType = DataType.KVAh;
                DataUnitOfMeasure = DataUnitOfMeasure.KVAh;
                break;
            case "G":
                MeterType = MeterType.Master;
                DataType = DataType.PowerFactor;
                DataUnitOfMeasure = DataUnitOfMeasure.pF;
                break;
            case "H":
                MeterType = MeterType.Master;
                DataType = DataType.QMetering;
                DataUnitOfMeasure = DataUnitOfMeasure.Qh;
                break;
            case "Y":
                MeterType = MeterType.Check;
                DataType = DataType.QMetering;
                DataUnitOfMeasure = DataUnitOfMeasure.Qh;
                break;
            case "M":
                MeterType = MeterType.Master;
                DataType = DataType.ParMetering;
                DataUnitOfMeasure = DataUnitOfMeasure.parh;
                break;
            case "W":
                MeterType = MeterType.Check;
                DataType = DataType.ParMetering;
                DataUnitOfMeasure = DataUnitOfMeasure.parh;
                break;
            case "V":
                MeterType = MeterType.Master;
                DataType = DataType.VoltsOrAmps;
                DataUnitOfMeasure = DataUnitOfMeasure.VA2h;
                break;
            case "Z":
                MeterType = MeterType.Check;
                DataType = DataType.VoltsOrAmps;
                DataUnitOfMeasure = DataUnitOfMeasure.VA2h;
                break;
            case "N":
                MeterType = MeterType.Net;
                DataType = DataType.Net;
                DataUnitOfMeasure = DataUnitOfMeasure.kWh;
                break;
            case "X":
                MeterType = MeterType.Net;
                DataType = DataType.Net;
                DataUnitOfMeasure = DataUnitOfMeasure.kvar;
                break;
            default:
                throw new InvalidDataException("Unable to parse Data Stream Suffix.");
        }
    }

    public string MeterCode { get; }
    public string MeterNumber { get; private set; }


    public MeterType MeterType { get; private set; }
    public DataUnitOfMeasure DataUnitOfMeasure { get; private set; }
    public DataType DataType { get; private set; }
}