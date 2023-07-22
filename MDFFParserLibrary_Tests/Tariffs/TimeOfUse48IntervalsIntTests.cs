using System.Diagnostics;
using MDFFParserLibrary.Models.Tariffs;
using NUnit.Framework;

namespace MDFFParserLibrary_Tests.Tariffs;

[TestFixture]
public class TimeOfUse48IntervalsIntTests
{
    private static TimeOfUseRate? _timeOfUse;

    private const int IntervalsPerDay = 48;
    private const int MinInIntervalsPerDay = 30;

    private const string StrPeak = "Peak";
    private const string StrOffPeak = "Off-Peak";
    private const string StrShoulder = "Shoulder";
    
    [SetUp]
    public void Setup()
    {
        _timeOfUse = new TimeOfUseRate(IntervalsPerDay);
        
        // NOTE: because this is using half hour intervals, the interval is double what you see in the 24 intervals tests!
        
        // Peak 6am-10am
        _timeOfUse.SetTariff(StrPeak,12, 20, 0.44539m);
        
        // Peak 3pm-1am
        _timeOfUse.SetTariff(StrPeak,30, 48, 0.44539m);
        _timeOfUse.SetTariff(StrPeak,0, 2, 0.44539m);
        
        // Off-peak 1am-6am 
        _timeOfUse.SetTariff(StrOffPeak,2, 12, 0.30734m);
        
        // Shoulder 10am-3pm
        _timeOfUse.SetTariff(StrShoulder,20, 30, 0.24431m);
    }
    
    
    [Test]
    public void CheckSetup()
    {
        Assert.That(_timeOfUse, Is.Not.Null);
        Assert.That(_timeOfUse?.IntervalsPerDay, Is.EqualTo(IntervalsPerDay));
        Assert.That(_timeOfUse?.MinsPerIntervalInDay, Is.EqualTo(MinInIntervalsPerDay));
    }

    [Test]
    public void CheckNumberOfIntervals()
    {
        Assert.That(_timeOfUse, Is.Not.Null);
        Assert.That(_timeOfUse?.TariffPerInterval, Is.Not.Null);
        Assert.That(_timeOfUse?.TariffPerInterval.Length, Is.EqualTo(IntervalsPerDay));
        
        Assert.That(_timeOfUse?.TariffNamePerInterval, Is.Not.Null);
        Assert.That(_timeOfUse?.TariffNamePerInterval.Length, Is.EqualTo(IntervalsPerDay));
    }
    
    [Test]
    public void CheckGeneratedTariffs()
    {
        var expectedNames = new[]
        {
            0.44539m, 0.44539m,// 24-1
            0.30734m, 0.30734m, 0.30734m, 0.30734m, 0.30734m, 0.30734m, 0.30734m, 0.30734m, 0.30734m, 0.30734m,// 1-6
            0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m,// 6-10
            0.24431m, 0.24431m, 0.24431m, 0.24431m, 0.24431m, 0.24431m, 0.24431m, 0.24431m, 0.24431m, 0.24431m,// 10-15
            0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m, 0.44539m // 15-24
        };
        
        Assert.That(_timeOfUse, Is.Not.Null);
        Assert.That(_timeOfUse?.TariffPerInterval, Is.EqualTo(expectedNames));
    }
    
    [Test]
    public void CheckGeneratedNames()
    {
        var expectedNames = new[]
        {
            StrPeak, StrPeak, // 24-1
            StrOffPeak, StrOffPeak, StrOffPeak, StrOffPeak, StrOffPeak, StrOffPeak, StrOffPeak, StrOffPeak, StrOffPeak, StrOffPeak,// 1-6
            StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak,// 6-10
            StrShoulder, StrShoulder, StrShoulder, StrShoulder, StrShoulder, StrShoulder, StrShoulder, StrShoulder, StrShoulder, StrShoulder, // 10-15
            StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak, StrPeak // 15-24
        };
        
        Assert.That(_timeOfUse, Is.Not.Null);
        Assert.That(_timeOfUse?.TariffNamePerInterval, Is.EqualTo(expectedNames));
    }
    
    [Test]
    public void CheckGetRateInt()
    {
        Assert.That(_timeOfUse, Is.Not.Null);

        Assert.That(_timeOfUse?.TariffNamePerInterval[10], Is.EqualTo(StrOffPeak));
        Assert.That(_timeOfUse?.GetRate(10), Is.EqualTo(0.30734m));
        
        Assert.That(_timeOfUse?.TariffNamePerInterval[12], Is.EqualTo(StrPeak));
        Assert.That(_timeOfUse?.GetRate(12), Is.EqualTo(0.44539m));
        
        Assert.That(_timeOfUse?.TariffNamePerInterval[36], Is.EqualTo(StrPeak));
        Assert.That(_timeOfUse?.GetRate(36), Is.EqualTo(0.44539m));
        
        Assert.That(_timeOfUse?.TariffNamePerInterval[47], Is.EqualTo(StrPeak));
        Assert.That(_timeOfUse?.GetRate(47), Is.EqualTo(0.44539m));
    }
}