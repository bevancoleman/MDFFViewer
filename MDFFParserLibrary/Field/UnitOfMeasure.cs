using MDFFParserLibrary.Models.Enums;

namespace MDFFParserLibrary.Field;

public static class UnitOfMeasure
{
    public static DataUnitOfMeasure Parse(string uom)
    {
        switch (uom.ToLower())
        {
            case "kvar":
                return DataUnitOfMeasure.kvar;
                break;
            case "kwh":
                return DataUnitOfMeasure.kWh;
                break;
            default:
                throw new ArgumentOutOfRangeException("Unable to parse Unit of Measure");
        }
    }
}