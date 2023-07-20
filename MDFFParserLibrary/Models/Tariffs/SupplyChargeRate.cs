namespace MDFFParserLibrary.Models.Tariffs;

public class SupplyChargeRate : IRate
{
    public decimal SupplyCharge { get; private set; }
    public int IntervalsPerDay { get; private set; }
    
    public decimal[] SupplyChargePerInterval { get; private set; }
    
    public SupplyChargeRate(int intervalsPerDay, decimal supplyChargePerDay)
    {
        IntervalsPerDay = intervalsPerDay;
        
        // Supply Charge, Fill with shared value
        SupplyCharge = supplyChargePerDay;
        var charge  = SupplyCharge / IntervalsPerDay;
        SupplyChargePerInterval = Enumerable.Repeat<decimal>(charge, IntervalsPerDay).ToArray();
    }

    public decimal GetRate(int interval)
    {
        return SupplyChargePerInterval[0];
    }

    public decimal GetRate(TimeSpan timeOfDay)
    {
        return GetRate(0);
    }

    public string GetName(int interval)
    {
        return "Supply Charge";
    }

    public string GetName(TimeSpan timeOfDay)
    {
        return GetName(0);
    }
}