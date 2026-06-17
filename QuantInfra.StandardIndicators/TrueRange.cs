using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Max value of Max((High - Low), (High - previous Close)) and (previous Close - Low)
/// </summary>
public class TrueRange : AbstractIndicator
{
    AbstractIndicator _high, _low, _close;

    public TrueRange(AbstractIndicator high = null,
        AbstractIndicator low = null,
        AbstractIndicator close = null)
    {
        _high = high?? new High();
        _low = low?? new Low();
        _close = close?? new Close();

        RegisterIndicator(_close, 1);
        RegisterIndicator(_low);
        RegisterIndicator(_high);

        Id = $"TrueRange({_high.Id},{_low.Id},{_close.Id})";
        IsSeparateWindow = true;
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var tmp = Math.Max((double)(_high.GetValue(bars[0]) - _low.GetValue(bars[0])), (double)(_high.GetValue(bars[0]) - _close.GetValue(bars[1])));
        return Math.Max(tmp, (double)(_close.GetValue(bars[1]) - _low.GetValue(bars[0])));
    }
}