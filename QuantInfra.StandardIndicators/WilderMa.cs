using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Wilder’s Moving Average is calculated using Wilder’s smoothing technique,
/// which gives greater weight to more recent data points. The calculation
/// involves subtracting the previous average from the current price and adding
/// the resulting difference to the previous average. This results in a faster
/// and more responsive moving average than other traditional methods. 
/// </summary>
public class WilderMA : AbstractIndicator
{
    int _period;
    AbstractIndicator _underlyingIndicator;

    public WilderMA(AbstractIndicator ind = null, int period = 9)
    {
        _period = period;
        _underlyingIndicator = ind ?? new Close();
        RegisterIndicator(_underlyingIndicator);

        Id = $"WilderMA:{_period}:{_underlyingIndicator.Id}";
        WarmupBars = _period;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var currentValue = _underlyingIndicator.GetValue(bars.CurrentBar);
        if (!currentValue.HasValue) return null;
        if (bars.Count < 2 || !GetValue(bars[1]).HasValue) return currentValue;
        return (currentValue + GetValue(bars[1]) * (_period - 1)) / _period;
    }
}