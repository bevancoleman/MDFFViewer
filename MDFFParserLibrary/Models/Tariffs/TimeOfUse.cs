namespace MDFFParserLibrary.Models.Tariffs;

public class TimeOfUse
{
    public int IntervalsPerDay { get; private set; }
    public decimal SupplyChargePerDay { get; private set; }
    
    public decimal[] TariffPerInterval { get; private set; }
    public string[] TariffNamePerInterval { get; private set; }
    
    public decimal[] SupplyChargePerInterval { get; private set; }
        
    public TimeOfUse(int intervalsPerDay, decimal supplyChargePerDay)
    {
        if (intervalsPerDay > 24 * 60)
        {
            throw new ArgumentException("intervalsPerDay can't be smaller than a min.");
        }
        IntervalsPerDay = intervalsPerDay;
        SupplyChargePerDay = supplyChargePerDay;

        // Supply Charge, Fill with shared value
        var charge = SupplyChargePerDay / IntervalsPerDay;
        SupplyChargePerInterval = Enumerable.Repeat<decimal>(charge, IntervalsPerDay).ToArray();
        
        // Tariff
        TariffPerInterval = new decimal[IntervalsPerDay];
        TariffNamePerInterval = new string[IntervalsPerDay];
    }
    
    public void SetTariff(string name, int fromInterval, int toInterval, decimal rateIncGst)
    {
        for (int i = fromInterval; i <= toInterval; i++)
        {
            TariffPerInterval[i] = rateIncGst;
            TariffNamePerInterval[i] = name;
        }
    }
    
    public void SetTariff(string name, TimeSpan fromInterval, TimeSpan toInterval, decimal rateIncGst)
    {
        throw new NotImplementedException();
        var fromIntervalInt = 0;
        var toIntervalInt = 0;
        SetTariff(name, fromIntervalInt, toIntervalInt, rateIncGst);
    }

    public decimal GetRate(int interval, bool includeSupply)
    {
        if (interval < 0 || interval >= TariffPerInterval.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(interval));
        }

        var result = TariffPerInterval[interval];
        if (includeSupply)
        {
            result += SupplyChargePerInterval[interval];
        }
        return result;
    }
    
    public decimal GetRate(TimeSpan timeOfDay, bool includeSupply)
    {
        var minsInInterval = 24 * 60 / IntervalsPerDay;
        var interval = (int)(timeOfDay.TotalMinutes) / minsInInterval;
        
        return GetRate(interval, includeSupply);
    }
}