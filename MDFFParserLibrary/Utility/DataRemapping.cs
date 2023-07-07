using MDFFParserLibrary.Consts;
using MDFFParserLibrary.Models.Enums;

namespace MDFFParserLibrary.Utility;

public static class DataRemapping
{
    public static IEnumerable<decimal> RemapValues(decimal[] nemIntervalValue, int graphInterval, int nemInterval,
        DataUnitOfMeasure graphUom, DataUnitOfMeasure nemUom)
    {
        if (graphInterval < nemInterval)
            throw new NotImplementedException("Not implemented ability to resample upwards.");

        if (graphUom != nemUom)
            throw new NotImplementedException("Not implemented ability to convert Unit of Measure.");


        var ret = new decimal[Intervals.MinsInDay / graphInterval];
        var multiple = graphInterval / nemInterval;

        var nemS = 0;
        var nemE = 0;
        for (var i = 0; i < ret.Length; i++)
        {
            var sumVal = 0m;
            nemE += multiple;

            while (nemS < nemE)
            {
                sumVal += nemIntervalValue[nemS];
                nemS++;
            }

            ret[i] = sumVal;
        }

        return ret;
    }
}