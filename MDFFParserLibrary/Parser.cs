﻿using MDFFParserLibrary.Field;
using MDFFParserLibrary.Line;
using MDFFParserLibrary.Models;
using MDFFParserLibrary.Models.Enums;
using MDFFParserLibrary.Models.Graph;
using MDFFParserLibrary.Models.IntervalMeteringData;
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


    public BasicGraph ConvertToSeries(IEnumerable<BaseNem> nemFile, DateTime graphFrom, DateTime graphTo,
        int graphInterval, DataUnitOfMeasure graphUom)
    {
        // make sure date only
        graphFrom = graphFrom.Date;
        graphTo = graphTo.Date;

        if (graphFrom > graphTo) throw new ArgumentOutOfRangeException("From has to be less than or equal To date.");

        var results = new BasicGraph();

        // Work out Axis buckets
        results.Axis = new AxisDateTime
        {
            Name = "Timeline"
        };

        var bucket = graphFrom;
        while (bucket < graphTo.AddDays(1))
        {
            results.Axis.Values.Add(bucket.ToString(), bucket);
            bucket += new TimeSpan(0, graphInterval, 0);
        }

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
                    if (!results.DataSeries.TryGetValue(seriesName, out currentSeries))
                    {
                        currentSeries = new SeriesDecimal(seriesName);
                        results.DataSeries.Add(seriesName, currentSeries);
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
}