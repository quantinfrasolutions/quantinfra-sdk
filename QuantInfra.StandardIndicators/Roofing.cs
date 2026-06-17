using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Implementation of stohastic based on Digital Filters course.
/// HighPassFilter6Db averaged with SuperSmoother.
/// </summary>
public class Roofing : AbstractIndicator
{
    AbstractIndicator _superSmooth;
 
    public Roofing(AbstractIndicator source = null, int hpPeriod = 7, int ssPeriod = 7)
    {
        source = source ?? new Close();
        var hp = new HighPassFilter6Db(source, hpPeriod);
        _superSmooth = new SuperSmoother(hp, ssPeriod);

        RegisterIndicator(_superSmooth);

        Id = $"Roofing:{hpPeriod}:{ssPeriod}({source.Id})";
        IsSeparateWindow = true;
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return _superSmooth.GetValue(bars.CurrentBar);
    }
}