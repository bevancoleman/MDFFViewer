using MDFFParserLibrary.Consts;
using MDFFParserLibrary.Models.Enums;

namespace MDFFParserLibrary.Utility;

public static class DataRemapping
{
    // Remap decimal array into another. Used when Intervals are different
    public static decimal[] RemapValues(decimal[] decimalArray, int graphInterval, int nemInterval)
    {
        if (graphInterval < nemInterval)
            throw new NotImplementedException("Not implemented ability to resample upwards.");
        
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
                sumVal += decimalArray[nemS];
                nemS++;
            }

            ret[i] = sumVal;
        }

        return ret;
    }
}