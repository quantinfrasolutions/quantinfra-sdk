using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Weighted moving average. Weights are distributed in a descending order.
/// </summary>
public class WMA : AbstractIndicator
{
    int _period;
    AbstractIndicator _underlyingIndicator;

    public WMA(AbstractIndicator ind, int period = 9)
    {
        _period = period;
        _underlyingIndicator = ind ?? new Close();

        RegisterIndicator(_underlyingIndicator, _period - 1);

        Id = $"WMA:{_period}({_underlyingIndicator.Id})";
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        double denominator = _period * (_period + 1) / 2;
        double numerator = 0;
        int f = _period;

        for (int i = 0; i < _period; i++)
        {
            numerator+= (double)_underlyingIndicator.GetValue(bars[i]) * f;
            f--;
        }

        return numerator / denominator;
    }
}