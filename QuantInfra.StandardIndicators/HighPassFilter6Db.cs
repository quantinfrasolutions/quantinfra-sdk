using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Implementation of stohastic based on Digital Filters course.
/// Removes - 6 dB per octave. It has less latency, but does not remove spectral extension.
/// Single-pole filter formula. The classic high-pass filter and momentum remove
/// the trend component of the price
/// </summary>
public class HighPassFilter6Db : AbstractIndicator
{
    AbstractIndicator _source;
    int _period;
    double _b0, _b1, _a1;

    public HighPassFilter6Db(AbstractIndicator source = null, int period = 7)
    {
        _period = period;
        _source = source ?? new Close();
        double angle = 2 * Math.PI / _period; // Угол в радианах (в формулах Джона Элерса угол в градусах)
        double alpha = (1 - Math.Sin(angle)) / Math.Cos(angle);

        _b0 = (1d + alpha) / 2d;
        _b1 = -(1d + alpha) / 2d;
        _a1 = -alpha;

        RegisterIndicator(_source, 1);

        Id = $"HighPassFilter6Db:{_period}({_source.Id})";
        IsSeparateWindow = true;
        WarmupBars = 1;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var currentValue = _source.GetValue(bars.CurrentBar);
        if (!currentValue.HasValue) return null;
        var previousValue = GetValue(bars[1]);
        if (bars.Count < 2 || !GetValue(bars[1]).HasValue) return currentValue;
        
        return _b0 * currentValue + _b1 * _source.GetValue(bars[1]) - _a1 * previousValue; 
    }
}