using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>  
/// Heikin-Ashi, also sometimes spelled Heiken-Ashi, means "average bar" in Japanese.
/// High is calculated as the max of open,close and high.
/// </summary>
public class HeikinAshiHigh : AbstractIndicator
{
    AbstractIndicator _close, _open;
    public HeikinAshiHigh()
    {
        _close = new HeikinAshiClose();
        _open = new HeikinAshiOpen();
        RegisterIndicator(_close);
        RegisterIndicator(_open);

        Id = $"HeikinAshiHigh";
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return new[] { bars[0].High, _close.GetValue(bars[0]), _open.GetValue(bars[0]) }.Max();
    }

}