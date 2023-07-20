namespace MDFFParserLibrary.Models.Tariffs;

public class TimeOfUseRate : IRate
{
    public int IntervalsPerDay { get; private set; }
    public decimal[] TariffPerInterval { get; private set; }
    public string[] TariffNamePerInterval { get; private set; }
    
    public int MinsPerIntervalInDay { get; private set; }
        
    public TimeOfUseRate(int intervalsPerDay)
    {
        if (intervalsPerDay > 24 * 60)
        {
            throw new ArgumentException("intervalsPerDay can't be smaller than a min.");
        }
        IntervalsPerDay = intervalsPerDay;
        
        // Tariff
        TariffPerInterval = new decimal[IntervalsPerDay];
        TariffNamePerInterval = new string[IntervalsPerDay];
        
        // Set Min per Interval in day
        MinsPerIntervalInDay = 24 * 60 / IntervalsPerDay;
    }
    
    public void SetTariff(string name, int fromInterval, int toInterval, decimal rateIncGst)
    {
        for (int i = fromInterval; i < toInterval; i++)
        {
            TariffPerInterval[i] = rateIncGst;
            TariffNamePerInterval[i] = name;
        }
    }
    
    public void SetTariff(string name, TimeSpan fromInterval, TimeSpan toInterval, decimal rateIncGst)
    {
        var fromIntervalInt = (int)(fromInterval.TotalMinutes) / MinsPerIntervalInDay;
        var toIntervalInt = (int)(toInterval.TotalMinutes) / MinsPerIntervalInDay;;
        SetTariff(name, fromIntervalInt, toIntervalInt, rateIncGst);
    }

    public decimal GetRate(int interval)
    {
        if (interval < 0 || interval >= TariffPerInterval.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(interval));
        }
        return TariffPerInterval[interval];
    }
    
    public decimal GetRate(TimeSpan timeOfDay)
    {
        var interval = (int)(timeOfDay.TotalMinutes) / MinsPerIntervalInDay;
        return GetRate(interval);
    }

    public string GetName(int interval)
    {
        return TariffNamePerInterval[interval];
    }

    public string GetName(TimeSpan timeOfDay)
    {
        var interval = (int)(timeOfDay.TotalMinutes) / MinsPerIntervalInDay;
        return GetName(interval);
    }
}