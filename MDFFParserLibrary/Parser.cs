using System.Diagnostics.Tracing;
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

        var intervalsPerDay = 24 * 60 / graphInterval;

        results.Axis = GenerateAxis(graphFrom, graphTo, graphInterval);
        results.DataSeries = GenerateDataSeries(nemFile, graphFrom, graphTo, graphInterval, graphUom);
        results.ValueSeries = GenerateValueSeries(results.DataSeries, intervalsPerDay);

        return results;
    }

    private Dictionary<string, SeriesEnergy> GenerateValueSeries(Dictionary<string, SeriesEnergy> resultsDataSeries, int intervalsPerDay)
    {
        var result = new Dictionary<string, SeriesEnergy>();

        // Setup main rate
        var mainExportRates = new TimeOfUseRate(intervalsPerDay);
        const string StrPeak = "Peak";
        const string StrOffPeak = "Off-Peak";
        const string StrShoulder = "Shoulder";
        
        // - Peak 6am-10am
        mainExportRates.SetTariff(StrPeak,new TimeSpan(6,0,0), new TimeSpan(10,0,0), 0.44539m);
        
        // - Peak 3pm-1am
        mainExportRates.SetTariff(StrPeak,new TimeSpan(15,0,0), new TimeSpan(24,0,0), 0.44539m);
        mainExportRates.SetTariff(StrPeak,new TimeSpan(0,0,0), new TimeSpan(1,0,0), 0.44539m);
        
        // - Off-peak 1am-6am 
        mainExportRates.SetTariff(StrOffPeak,new TimeSpan(1,0,0), new TimeSpan(6,0,0), 0.30734m);
        
        // - Shoulder 10am-3pm
        mainExportRates.SetTariff(StrShoulder,new TimeSpan(10,0,0), new TimeSpan(15,0,0), 0.24431m);
        
        // Setup Controlled Rate
        var controlledLoadRates = new SingleRate(intervalsPerDay, 0m);
        // TODO - Setup Controlled Rate
        
        // Setup Solar Export Rate
        var solarRates = new VolumeRate();
        // TODO - Setup Solar Export Rate
        
        // Setup Supply Charge
        var supplyCharge = new SupplyChargeRate(intervalsPerDay, 0.88429m);

        // For each series line, create a new value series, applying to relevant rate
        foreach (var data in resultsDataSeries)
        {
            switch (data.Key)
            {
                case "1234567890/E1 (Export)":
                    result.Add("E1", CalculateValueSeries("E1", data.Value, mainExportRates) );
                    break;
                
                case "1234567890/B1 (Import)":
                    //result.Add("B1", CalculateValueSeries(data, solarRates) );  
                    break;
                
                case "1234567890/E2 (Export)":
                    //result.Add("E2", CalculateValueSeries(data, mainExportRates) );                    
                    break;
                
                default:
                    throw new Exception("Unmapped meter");
            }
        }
        return result;
    }

    private SeriesEnergy CalculateValueSeries(string name, SeriesEnergy data, IRate rate)
    {
        var result = new SeriesEnergy(name);

        foreach (var d in data.Values)
        {
            result.Values.Add(new SeriesEnergy.EnergyValue()
            {
                Day = d.Day,
                Year = d.Year,
                Interval = d.Interval,
                Value = d.Value * rate.GetRate(d.Interval)
            });
        }
        return result;
    }


    private static Dictionary<string, SeriesEnergy> GenerateDataSeries(IEnumerable<BaseNem> nemFile, DateTime graphFrom, DateTime graphTo, int graphInterval, DataUnitOfMeasure graphUom)
    {
        var results = new Dictionary<string, SeriesEnergy>();
        
        SeriesEnergy currentSeries = null;
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
                        currentSeries = new SeriesEnergy(seriesName);
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
                    currentSeries.Values.AddRange(ConvertToEnergySeries(datarecord));
                else
                    // If not... remap
                    currentSeries.Values.AddRange(RemapToEnergySeries(datarecord, graphInterval, nemInterval, graphUom, nemUom));
            }
        }

        return results;
    }

    private static IEnumerable<SeriesEnergy.EnergyValue> ConvertToEnergySeries(IntervalDataRecord300 record300)
    {
        var l = record300.IntervalValue.Length;
        var result = new SeriesEnergy.EnergyValue[l];

        for (var i = 0; i < l; i++)
        {
            result[i] = new SeriesEnergy.EnergyValue
            {
                Year = record300.IntervalDate.Year,
                Day = record300.IntervalDate.DayOfYear,
                Interval = i,
                Value = record300.IntervalValue[i]
            };
        }
        return result;
    }
    
    private static IEnumerable<SeriesEnergy.EnergyValue> RemapToEnergySeries(IntervalDataRecord300 record300,  int graphInterval, int nemInterval, DataUnitOfMeasure graphUom, DataUnitOfMeasure nemUom)
    {
        if (graphUom != nemUom)
            throw new NotImplementedException("Not implemented ability to convert Unit of Measure.");

        var remappedData = DataRemapping.RemapValues(record300.IntervalValue, graphInterval, nemInterval);

        var newRecrd300 = new IntervalDataRecord300(record300.IntervalDate, remappedData,
            record300.QualityMethod, record300.ReasonCode, record300.ReasonDescription, record300.UpdateDateTime,
            record300.MSATSLoadDateTime);

        return ConvertToEnergySeries(newRecrd300);
    }

    private static AxisDateTime GenerateAxis(DateTime graphFrom, DateTime graphTo, int intervalMins)
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
            bucket += new TimeSpan(0, intervalMins, 0);
        }

        return result;
    }
}