using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Moving average convergence/divergence (MACD, or MAC-D) is a trend-following
/// momentum indicator that shows the relationship between two exponential
/// moving averages (EMAs) of a security’s price. The MACD line is calculated
/// by subtracting the long-period EMA from the short-period EMA.
/// </summary>
public class Macd : AbstractIndicator
{
    EMA _emaFirst, _emaSecond;

    public Macd(AbstractIndicator source = null, int periodFirst = 12, int periodSecond = 26)
    {
        source = source ?? new Close();
        _emaFirst = new EMA(source, periodFirst);
        _emaSecond = new EMA(source, periodSecond);
            
        RegisterIndicator(_emaFirst);
        RegisterIndicator(_emaSecond);

        Id = $"MACD:{periodFirst}:{periodSecond}({source.Id})";
        IsSeparateWindow = true;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
        => _emaFirst.GetValue(bars[0]) - _emaSecond.GetValue(bars[0]);
}