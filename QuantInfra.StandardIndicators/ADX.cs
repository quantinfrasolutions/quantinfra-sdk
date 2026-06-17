using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// ADX is used to quantify trend strength.ADX calculations are based on
/// a moving average of price range expansion over a given period of time.
/// </summary>
public class ADX : AbstractIndicator
{
    int _period;
    WilderMA _ma;

    public ADX(AbstractIndicator high = null, AbstractIndicator low = null,
        AbstractIndicator close = null, int period = 7)
    {
        _period = period;
        high = high ?? new High();
        low = low ?? new Low();
        close = close ?? new Close();
        _ma = new WilderMA(new DX(high, low, close, period), period);

        RegisterIndicator(_ma);

        Id = $"ADX:{_period}({high.Id},{low.Id},{close.Id})";
        IsSeparateWindow = true;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return _ma.GetValue(bars[0]);
    }
}