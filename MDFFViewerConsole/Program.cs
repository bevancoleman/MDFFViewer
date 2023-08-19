using System.Globalization;
using CommandLine;
using CsvHelper;
using MDFFParserLibrary.Models.Enums;
using Parser = MDFFParserLibrary.Parser;

namespace MDFFViewerConsole;

internal class Program
{
    private static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunOptions)
            .WithNotParsed(HandleParseError);
    }

    static void RunOptions(Options opts)
    {
        // Check In File
        if (!File.Exists(opts.InFile))
        {
            throw new ArgumentException("Input file - No file, or invalid file specified.");
        }

        // Check Out File
        if (!CheckPathIsProbablyValid(opts.OutFile))
        {
            throw new ArgumentException("Output file - Path not valid.");
        }

        // Read File
        var parser = new Parser();
        var results = parser.ReadFile(opts.InFile);

        // Process File and Convert to Series
        var dateFrom = new DateTime(2023, 05, 01);
        var dateTo = new DateTime(2023, 05, 01);

        //TODO : Need to work out how to convert this into a format which is easier to work with a CSV, this isn't working great :(
        var seriesData = parser.ConvertToSeries(results, dateFrom, dateTo, 30, DataUnitOfMeasure.kWh);

        // Output to CSV. See https://joshclose.github.io/CsvHelper/
        using (var writer = new StreamWriter(opts.OutFile))
        {
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // Header
                foreach (var field in seriesData.DataSeries.Keys)
                {
                    csv.WriteField(field);
                }

                foreach (var field in seriesData.ValueSeries.Keys)
                {
                    csv.WriteField(field);
                }

                csv.NextRecord();

                // Records
                foreach (var row in seriesData.Axis.Values)
                {
                    csv.WriteField(row);
                    foreach (var meter in seriesData.DataSeries.Keys)
                    {
                        csv.WriteField(seriesData.DataSeries[meter].Values);
                    }

                    csv.NextRecord();
                }
            }
        }
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
        foreach (var err in errs)
        {
            Console.Error.WriteLine(err);
        }
    }

    private static bool CheckPathIsProbablyValid(string path)
    {
        FileInfo fi = null;
        try
        {
            fi = new FileInfo(path);
        }
        catch (ArgumentException)
        {
        }
        catch (System.IO.PathTooLongException)
        {
        }
        catch (NotSupportedException)
        {
        }

        if (ReferenceEquals(fi, null))
        {
            // file name is not valid
            return false;
        }
        else
        {
            // file name is valid... May check for existence by calling fi.Exists.
            return true;
        }
    }

    /// <summary>
    /// Command line options. See https://github.com/commandlineparser/commandline.
    /// </summary>
    public class Options
    {
        [Option('i', "infile", Required = true, HelpText = "Input NEM file.")]
        public string InFile { get; set; }

        [Option('o', "outfile", Required = true, HelpText = "Output CSV file.")]
        public string OutFile { get; set; }
    }
}