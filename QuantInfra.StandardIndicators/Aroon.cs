using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Aroon indicator measures the time between highs and the time between
/// lows over a time period. The idea is that strong uptrends will regularly
/// see new highs, and strong downtrends will regularly see new lows.
/// The indicator signals when this is happening, and when it isn't.
/// The indicator consists of the "Aroon up" line, which measures the strength
/// of the uptrend, and the "Aroon down" line, which measures the strength of the downtrend.
/// </summary>
public class Aroon : AbstractIndicator
{
    int _period;
    bool _isUp;
    AbstractIndicator _source;

    public Aroon(AbstractIndicator source = null, int period = 14, bool isUp = true)
    {
        _period = period;
        _source = source ?? new Close();
        _isUp = isUp;

        RegisterIndicator(_source, _period);

        Id = $"Aroon:{_period}:{_isUp}({_source.Id})";
        IsSeparateWindow = true;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        int res = _period;            
        double? tmp = _source.GetValue(bars[_period]);

        for (int i = _period - 1; i >= 0; i--)
        {
            if (_isUp ? (tmp < _source.GetValue(bars[i])) : (tmp > _source.GetValue(bars[i])))
            {
                res = i;
                tmp = _source.GetValue(bars[i]);
            }
        }
        return (_period - res) * 100 / _period;
    }
}