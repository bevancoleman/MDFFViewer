using MDFFParserLibrary.Field;
using MDFFParserLibrary.Line;
using MDFFParserLibrary.Models;
using MDFFParserLibrary.Models.Enums;
using MDFFParserLibrary.Models.Graph;
using MDFFParserLibrary.Models.IntervalMeteringData;
using MDFFParserLibrary.Models.Tariffs;
using MDFFParserLibrary.Utility;

namespace MDFFParserLibrary;

public class Parser
{
    // https://www.aemo.com.au/-/media/archive/files/other/consultations/nem/meter-data-file-format-specification-nem12-nem13/mdff-specification-nem12-nem13-final-v102-clean.pdf

    public BaseNem[] ReadFile(string fileName)
    {
        var records = new List<BaseNem>();

        foreach (var line in File.ReadLines(fileName))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue; //Skip empty line

            var lineSplit = line.Split(",");

            if (lineSplit.Length < 1)
                throw new ApplicationException("Input line not a CSV");

            switch (lineSplit[0])
            {
                case "100":
                    records.Add(ParseHeader(lineSplit));
                    break;

                case "200":
                    records.Add(new NmiDataDetails().ParseLine(lineSplit));
                    break;

                case "300":
                    records.Add(new IntervalDataRecord().ParseLine(lineSplit));
                    break;

                case "400":
                    break;

                case "500":
                    break;

                case "900":
                    records.Add(ParseFooter(lineSplit));
                    break;

                default:
                    throw new ApplicationException("Unknown Line");
            }
        }

        return records.ToArray();
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private Footer900 ParseFooter(string[] lineSplit)
    {
        if (lineSplit.Length != 1) throw new ApplicationException("Invalid Footer Record (length)");

        if (lineSplit[0] != "900") throw new ApplicationException("Invalid Footer Record (id)");

        return new Footer900();
    }

    private Header100 ParseHeader(string[] lineSplit)
    {
        if (lineSplit.Length != 5) throw new ApplicationException("Invalid Header Record (length)");

        if (lineSplit[0] != "100") throw new ApplicationException("Invalid Header Record (id)");

        return new Header100(lineSplit[1], Dates.ParseDate(lineSplit[2]).Value, lineSplit[3], lineSplit[4]);
    }


    public BasicGraph ConvertToSeries(IEnumerable<BaseNem> nemFile, DateTime graphFrom, DateTime graphTo, int graphInterval, DataUnitOfMeasure graphUom)
    {
        // make sure date only
        graphFrom = graphFrom.Date;
        graphTo = graphTo.Date;

        if (graphFrom > graphTo) throw new ArgumentOutOfRangeException("From has to be less than or equal To date.");

        var results = new BasicGraph();

        results.Axis = GenerateAxis(graphFrom, graphTo, graphInterval);
        results.DataSeries = GenerateDataSeries(nemFile, graphFrom, graphTo, graphInterval, graphUom);
        results.ValueSeries = GenerateValueSeries(results.DataSeries, graphInterval);

        return results;
    }

    private Dictionary<string, SeriesDecimal> GenerateValueSeries(Dictionary<string, SeriesDecimal> resultsDataSeries, int intervals)
    {
        var result = new Dictionary<string, SeriesDecimal>();

        // Setup main rate
        var mainExportRates = new TimeOfUseRate(intervals);
        const string StrPeak = "Peak";
        const string StrOffPeak = "Off-Peak";
        const string StrShoulder = "Shoulder";
        
        // - Peak 6am-10am
        mainExportRates.SetTariff(StrPeak,6, 10, 0.44539m);
        
        // - Peak 3pm-1am
        mainExportRates.SetTariff(StrPeak,15, 24, 0.44539m);
        mainExportRates.SetTariff(StrPeak,0, 1, 0.44539m);
        
        // - Off-peak 1am-6am 
        mainExportRates.SetTariff(StrOffPeak,1, 6, 0.30734m);
        
        // - Shoulder 10am-3pm
        mainExportRates.SetTariff(StrShoulder,10, 15, 0.24431m);
        
        // Setup Controlled Rate
        var controlledLoadRates = new SingleRate(intervals, 0m);
        // TODO - Setup Controlled Rate
        
        // Setup Solar Export Rate
        // TODO - Setup Solar Export Rate
        
        // Setup Supply Charge
        var supplyCharge = new SupplyChargeRate(intervals, 0.88429m);

        // For each series line, create a new value series, applying to relevant rate
        foreach (var data in resultsDataSeries)
        {
            switch (data.Key)
            {
                case "1234567890/E1 (Export)":
                    
                    break;
                case "1234567890/B1 (Import)":
                    
                    break;
                case "1234567890/E2 (Export)":
                    
                    break;
                default:
                    throw new Exception("Unmapped meter");
            }
            
        }

        return result;
    }

    private static Dictionary<string, SeriesDecimal> GenerateDataSeries(IEnumerable<BaseNem> nemFile, DateTime graphFrom, DateTime graphTo, int graphInterval, DataUnitOfMeasure graphUom)
    {
        var results = new Dictionary<string, SeriesDecimal>();
        
        SeriesDecimal currentSeries = null;
        var nemInterval = -1;
        var nemUom = DataUnitOfMeasure.Unknown;

        foreach (var nemRecord in nemFile)
        {
            if (nemRecord.RecordIndicator == 200)
            {
                // NMI Data Details (start of series)
                var nemDataDetail = (NmiDataDetails200)nemRecord;

                var dss = new DataStreamSuffix(nemDataDetail.NMISuffix);

                var seriesName = $@"{nemDataDetail.NMI}/{nemDataDetail.NMISuffix} ({dss.DataType})";
                nemInterval = nemDataDetail.IntervalLength;
                nemUom = nemDataDetail.UOM;

                if (currentSeries == null || currentSeries.Name != seriesName)
                    // Not current series, check if in collection or make new one (and add it)
                    if (!results.TryGetValue(seriesName, out currentSeries))
                    {
                        currentSeries = new SeriesDecimal(seriesName);
                        results.Add(seriesName, currentSeries);
                    }
            }
            else if (nemRecord.RecordIndicator == 300)
            {
                if (currentSeries == null)
                    throw new InvalidDataException(
                        "Have got a Data Record (300) before Data Details (200). This is out of order.");

                // NMI Interval Data Record
                var datarecord = (IntervalDataRecord300)nemRecord;

                // Check range
                if (datarecord.IntervalDate < graphFrom || datarecord.IntervalDate > graphTo)
                    continue;

                // Check if match to desired interval and unit of meassure
                if (graphInterval == nemInterval && nemUom == graphUom)
                    // If so... just use
                    currentSeries.Values.AddRange(datarecord.IntervalValue);
                else
                    // If not... remap
                    currentSeries.Values.AddRange(DataRemapping.RemapValues(datarecord.IntervalValue, graphInterval,
                        nemInterval, graphUom, nemUom));
            }
        }

        return results;
    }

    private static AxisDateTime GenerateAxis(DateTime graphFrom, DateTime graphTo, int graphInterval)
    {
        // Work out Axis buckets
        var result = new AxisDateTime
        {
            Name = "Timeline"
        };

        var bucket = graphFrom;
        while (bucket < graphTo.AddDays(1))
        {
            result.Values.Add(bucket.ToString(), bucket);
            bucket += new TimeSpan(0, graphInterval, 0);
        }

        return result;
    }
}