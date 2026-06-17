using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para> 
/// Simple implementation of linear regression of underlying.
/// </summary>
public class LinearRegression : AbstractIndicator
{
    int _period;
    AbstractIndicator _source;

    public LinearRegression(AbstractIndicator source = null, int period = 9)
    {
        _source = source ?? new High();
        _period  = period;

        RegisterIndicator(_source, period - 1);
        Id = $"LinearRegression:{_period}({_source.Id})";
        IsSeparateWindow = true;
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        double e = _period / 2;
        double f = (_period + 1) / 3;
        double? c = 2 * _source.GetValue(bars[_period]);
        double? d = 3 * _source.GetValue(bars[_period]);
        for(int i=0; i < _period; i++)
        {
            d += (2 * _source.GetValue(bars[i]) -c)/f;
            c += (_source.GetValue(bars[i]) - _source.GetValue(bars[_period - i]))/e;
        }
        return d-c;
    }
}