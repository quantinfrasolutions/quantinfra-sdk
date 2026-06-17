using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// PivotPointUp detects if current high is higher, than highest value for n periods.
/// </summary>
public class PivotPointUp : AbstractIndicator
{
    Highest highest;
    AbstractIndicator _source;

    public PivotPointUp(AbstractIndicator source = null, int period = 9)
    {
        _source = source ?? new High();
        highest = new Highest(_source, period);

        RegisterIndicator(highest, 1);

        Id = $"PivotPointUp:{period}({_source.Id})";
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return _source.GetValue(bars[0]) >= highest.GetValue(bars[1]) ? 1 : 0;
    }
}