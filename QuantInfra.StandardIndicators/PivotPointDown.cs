using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// PivotPointDown detects if current low is higher, than lowest value for n periods.
/// </summary>
public class PivotPointDown : AbstractIndicator
{
    Lowest _lowest;
    AbstractIndicator _source;

    public PivotPointDown(AbstractIndicator source = null, int period = 9)
    {
        _source = source ?? new Low();
        _lowest =  new Lowest(_source,  period);

        RegisterIndicator(_lowest, 1);
        RegisterIndicator(_source);

        Id = $"PivotPointDown:{period}({_source.Id})";
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return _source.GetValue(bars[0]) <= _lowest.GetValue(bars[1]) ? 1 : 0;
    }
}