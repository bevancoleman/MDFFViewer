using MDFFParserLibrary;
using MDFFParserLibrary.Models.Enums;

namespace MDFFViewerConsole;

internal class Program
{
    private static void Main(string[] args)
    {
        // Check File
        if (args.Length != 1 || !File.Exists(args[0]))
        {
            throw new ArgumentException("No File, or invalid file specified");
        }
        var file = args[0];
            
        // Read File
        var parser = new Parser();
        var results = parser.ReadFile(file);

        var seriesData = parser.ConvertToSeries(results, new DateTime(2023, 05, 01), new DateTime(2023, 05, 01), 30,
            DataUnitOfMeasure.kWh);

        /*foreach (var result in results)
        {
            Console.WriteLine(seriesData);
        }*/
        Console.WriteLine(seriesData);
    }
}