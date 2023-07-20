namespace MDFFParserLibrary.Models.Tariffs;

public class SingleRate : IRate
{
    public decimal SupplyCharge { get; private set; }
    public int IntervalsPerDay { get; private set; }
    
    public decimal SupplyChargePerInterval { get; private set; }
    
    public SingleRate(int intervalsPerDay, decimal supplyChargePerDay)
    {
        IntervalsPerDay = intervalsPerDay;
        SupplyCharge = supplyChargePerDay;
        SupplyChargePerInterval = SupplyCharge / IntervalsPerDay;
    }

    public decimal GetRate(int interval)
    {
        throw new NotImplementedException();
    }

    public decimal GetRate(TimeSpan timeOfDay)
    {
        throw new NotImplementedException();
    }

    public string GetName(int interval)
    {
        throw new NotImplementedException();
    }

    public string GetName(TimeSpan timeOfDay)
    {
        throw new NotImplementedException();
    }
}