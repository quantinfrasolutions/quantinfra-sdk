using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para> 
/// Chaikin Money Flow (CMF) is a volume-weighted average of A/D line
/// over a specified period. The standard CMF period is 21 days.
/// The principle behind the Chaikin Money Flow is the nearer the closing price
/// is to the high, the more accumulation has taken place.
/// Conversely, the nearer the closing price is to the low,
/// the more distribution has taken place.
/// </summary>
public class CMF : AbstractIndicator
{
    int _period;
    AbstractIndicator _high;
    AbstractIndicator _low;
    AbstractIndicator _close;
    AbstractIndicator _volume;

    public CMF(
        AbstractIndicator high = null,
        AbstractIndicator low = null,
        AbstractIndicator close = null,
        int period = 10)
    {
        _period = period;
        _high = high ?? new High();
        _low = low ?? new Low();
        _close = close ?? new Close();
        _volume = new Volume();

        RegisterIndicator(_volume, _period - 1);
        RegisterIndicator(_high, _period - 1);
        RegisterIndicator(_low, _period - 1);
        RegisterIndicator(_close, _period - 1);

        Id = $"CMF:{_period}({_high.Id},{_low.Id},{_close.Id})";
        IsSeparateWindow = true;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var volumes = bars.Take(_period).Sum(_volume.GetValue);
        var sum = bars
            .Take(_period)
            .Select(b =>
                (
                    (_close.GetValue(b) - _low.GetValue(b)) - (_high.GetValue(b) - _close.GetValue(b))
                )
                /
                (                        
                    _high.GetValue(b) - _low.GetValue(b)
                )
                * _volume.GetValue(b)
            ).Sum();
        return (double)(sum / volumes);
    }
}