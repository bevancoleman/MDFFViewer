namespace MDFFParserLibrary.Models.Tariffs;

public interface IRate
{
    public decimal GetRate(int interval);
    public decimal GetRate(TimeSpan timeOfDay);
    
    public string GetName(int interval);
    public string GetName(TimeSpan timeOfDay);
}