namespace MDFFParserLibrary.Models.Graph;

public class SeriesEnergy
{
    public class EnergyValue
    {
        public int Year { get; set; }
        public int Day { get; set; }
        public int Interval { get; set; }

        public decimal Value { get; set; }
    }

    public SeriesEnergy(string name)
    {
        Name = name;
        Values = new List<EnergyValue>();
    }

    public string Name { get; init; }
    public List<EnergyValue> Values { get; }
}