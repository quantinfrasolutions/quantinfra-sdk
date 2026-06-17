using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Implementation of stohastic based on Digital Filters course.
/// Removes -12 dB per octave. Completely removes spectral broadening,
/// but has longer latency.
/// Single-pole filter formula. The classic high-pass filter and momentum remove
/// the trend component of the price.
/// </summary>
public class HighPassFilter12Db : AbstractIndicator
{
    AbstractIndicator _source;
    int _period;
    double _b0, _b1, _a1, _b2, _a2;

    public HighPassFilter12Db(AbstractIndicator source = null, int period = 7)
    {
            
        _period = period;
        _source = source ?? new Close();
        double angle = Math.Sqrt(2) * Math.PI / _period; // Угол в радианах (в формулах Джона Элерса угол в градусах)
        double alpha = (Math.Cos(angle) + Math.Sin(angle) - 1) / Math.Cos(angle);

        _b0 = Math.Pow(1d - alpha / 2d, 2);
        _b1 = -2 * Math.Pow(1d - alpha / 2d, 2);
        _b2 = Math.Pow(1d - alpha / 2d, 2);
        _a1 = -2 * (1d - alpha);
        _a2 = Math.Pow(1d - alpha, 2); // Коэфф. числителя и знаменателя полинома передаточной функции (a0 = 1)

        RegisterIndicator(_source, 1);

        Id = $"HighPassFilter12Db:{_period}({_source.Id})";
        IsSeparateWindow = true;
        WarmupBars = 2;
    }
        

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var currentValue = _source.GetValue(bars.CurrentBar);
        if (!currentValue.HasValue) return null;

        var previousValue = GetValue(bars[1]);
        if (bars.Count < 2 || !GetValue(bars[1]).HasValue || !GetValue(bars[2]).HasValue) return currentValue;
        // Формула однополюсного фильтра. Классический фильтр ВЧ и моментум убирают трендовую составляющую цены
        return _b0 * currentValue + _b1 * _source.GetValue(bars[1]) + _b2 * _source.GetValue(bars[2])
               - _a1 * previousValue - _a2 * GetValue(bars[2]);
    }
}