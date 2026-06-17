using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Triple smoothed EMA is an underlying value. TRIX is calculated as follows:
/// (underlying - previous underlying)/underlying * 100
/// </summary>
public class TRIX : AbstractIndicator
{
    AbstractIndicator _underlyingIndicator;
    EMA _sma;
    EMA _doublesma;
    EMA _triplema;
    int _period;

    public TRIX(AbstractIndicator ind, int period = 9)
    {
        _period = period;
        _underlyingIndicator = ind ?? new Close();
        _sma = new EMA(_underlyingIndicator, _period);
        _doublesma = new EMA(_sma, _period);
        _triplema = new EMA(_doublesma, _period);
        RegisterIndicator(_triplema, 1);

        Id = $"TRIX:{_period}({_underlyingIndicator.Id})";
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return _triplema.GetValue(bars[0]) != 0 ? (_triplema.GetValue(bars[0]) - _triplema.GetValue(bars[1])) / _triplema.GetValue(bars[0]) * 100.0 : 0;
    }
}