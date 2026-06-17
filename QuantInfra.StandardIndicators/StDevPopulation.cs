using System;
using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>  
/// Implementation of standart deviation of underlying.
/// </summary> 
public class StDevPopulation : AbstractIndicator
{
    SimpleMovingAverage _sma;
    AbstractIndicator _source;
    int _period;

    public StDevPopulation(AbstractIndicator source = null, int period = 9)
    {
        _source = source ?? new Close();
        _period = period;
        _sma = new SimpleMovingAverage(_source, _period);
        RegisterIndicator(_source, _period - 1);
        RegisterIndicator(_sma);

        Id = $"StDevPopulation:{_period}({_source.Id})";
        IsSeparateWindow = true;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var avg = _sma.GetValue(bars[0]);
        double sum = bars.Take(_period).Select(b => Math.Pow((double)(_source.GetValue(b) - avg), 2)).Sum();
        return Math.Sqrt(sum / (_period - 1));
    }

}