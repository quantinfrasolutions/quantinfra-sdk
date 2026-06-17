using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>  
/// Heikin-Ashi, also sometimes spelled Heiken-Ashi, means "average bar" in Japanese.
/// Close is calculated as the average price of the current bar.
/// </summary>
public class HeikinAshiClose : AbstractIndicator
{
    public HeikinAshiClose()
    {
        Id = $"HeikinAshiClose";
    }

    protected override double? Calculate(IBarStorage bars, double? price = null) =>
        (bars[0].Close + bars[0].Open + bars[0].High + bars[0].Low) / 4;
}