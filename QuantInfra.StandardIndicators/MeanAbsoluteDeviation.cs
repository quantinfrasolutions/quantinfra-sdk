using System;
using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// The mean absolute deviation (MAD) is a measure of variability that indicates
/// the average distance between observations and their mean. 
/// </summary>
public class MeanAbsoluteDeviation : AbstractIndicator
{
    int _period;
    AbstractIndicator _source;
    SimpleMovingAverage _sma;

    public MeanAbsoluteDeviation(AbstractIndicator source = null, int period = 9)
    {
        _source = source ?? new Close();
        _period = period;
        _sma = new SimpleMovingAverage(_source, _period);
        RegisterIndicator(_sma);
        RegisterIndicator(_source, _period - 1);

        Id = $"MeanAbsoluteDeviation:{_period}({_source.Id})";
        IsSeparateWindow = true;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var average = _sma.GetValue(bars[0]);

        return bars
            .Take(_period)
            .Select(b => Math.Abs((_source.GetValue(b) ?? average.Value) - average.Value))
            .Average();
    }
}