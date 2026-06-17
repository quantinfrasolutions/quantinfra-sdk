using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>  
/// Average value of underlying for n periods.
/// </summary> 
public class SimpleMovingAverage : AbstractIndicator
{
    int _period;
    AbstractIndicator _source;

    public SimpleMovingAverage(AbstractIndicator source = null, int period = 9)
    {
        _source = source ?? new Close();
        _period = period;
        RegisterIndicator(_source, _period + 1);

        Id = $"SimpleMovingAverage:{_period}({_source.Id})";
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var previousValue = bars.Count > 1 ? GetValue(bars[1]) : null;
        if (!previousValue.HasValue) return bars.Take(_period).Select(_source.GetValue).Average();

        // this gives a performance boost
        return previousValue + _source.GetValue(bars.CurrentBar) / _period - _source.GetValue(bars[_period]) / _period;
    }
}