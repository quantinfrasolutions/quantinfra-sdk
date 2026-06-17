using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Helper for different indicators. Calculated as an average of High,Low and Close.
/// </summary>
public class HLC3 : AbstractIndicator
{
    AbstractIndicator _high, _low, _close;

    public HLC3(AbstractIndicator high = null,
        AbstractIndicator low = null,
        AbstractIndicator close = null)
    {
        _high = high ?? new High();
        _low = low ?? new Low();
        _close = close ?? new Close();

        RegisterIndicator(_high);
        RegisterIndicator(_low);
        RegisterIndicator(_close);

        Id = $"HLC3({_high.Id},{_low.Id},{_close.Id})";
        IsSeparateWindow = true;
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
        => (_high.GetValue(bars[0]) + _low.GetValue(bars[0]) + _close.GetValue(bars[0])) / 3;
}