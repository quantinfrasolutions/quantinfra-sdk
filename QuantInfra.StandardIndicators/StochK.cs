using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// StochK is a stochastic indicator, calculated as follows:
/// 100 * (Close - lowest(period))/ (highest(period)-lowest(period))
/// TODO: link Highest and lowest indicators
/// </summary>
public class StochK : AbstractIndicator
{
    int _numberOfPeriods;
    AbstractIndicator _high, _low, _close;

    public StochK(int numberOfPeriods = 14)
    {
        _high = new High();
        _low = new Low();
        _close = new Close();
        _numberOfPeriods = numberOfPeriods;

        RegisterIndicator(_low, _numberOfPeriods - 1);
        RegisterIndicator(_high, _numberOfPeriods - 1);
        RegisterIndicator(_close);

        Id = $"StochK:{_numberOfPeriods}";
        IsSeparateWindow = true;
        WarmupBars = _numberOfPeriods;
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var ln = bars.Take(_numberOfPeriods).Select(_low.GetValue).Min();
        var hn = bars.Take(_numberOfPeriods).Select(_high.GetValue).Max();

        return 100.0 * (_close.GetValue(bars[0]) - ln) / (hn - ln);
    }
}