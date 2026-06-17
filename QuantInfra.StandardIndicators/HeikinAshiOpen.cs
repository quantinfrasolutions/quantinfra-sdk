using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>  
/// Heikin-Ashi, also sometimes spelled Heiken-Ashi, means "average bar" in Japanese.
/// Open is calculated as the midpoint of the previous bar.​
/// </summary>
public class HeikinAshiOpen: AbstractIndicator
{
    AbstractIndicator _underlyingIndicator;
    public HeikinAshiOpen()
    {
        _underlyingIndicator = new HeikinAshiClose();
        RegisterIndicator(_underlyingIndicator, 1);

        Id = "HeikinAshiOpen";
        WarmupBars = 1;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        if (bars.Count < 2 || !GetValue(bars[1]).HasValue) return (bars[0].Open + _underlyingIndicator.GetValue(bars[0]))/2;
        return (GetValue(bars[1]) + _underlyingIndicator.GetValue(bars[1]))/2;
    }
            
}