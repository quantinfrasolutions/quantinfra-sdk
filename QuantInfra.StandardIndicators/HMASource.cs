using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
///  Helper for the Hull Moving Average (HMA). Calculated as 2* (WMA fast - WMA slow).
/// </summary>
public class HMASource : AbstractIndicator
{
    WMA _wmaFast;
    WMA _wmaSlow;

    public HMASource(AbstractIndicator ind, int period = 9)
    {
        ind = ind ?? new Close();
        _wmaFast = new WMA(ind, period/2);
        _wmaSlow = new WMA(ind, period);

        RegisterIndicator(_wmaFast);
        RegisterIndicator(_wmaSlow);

        Id = $"HMASource:{period}({ind.Id})";
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return 2 * _wmaFast.GetValue(bars[0])-_wmaSlow.GetValue(bars[0]);
    }
}