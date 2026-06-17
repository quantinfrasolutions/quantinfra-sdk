using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>  
/// SuperSmoother by John Ehlers (TASC 01.2014) removes all high frequencies     
/// John Ehlers writes that he worked with the Butterworth filter because
/// it has a smooth frequency response. Ehlers selected the formula to minimize delay
/// Therefore, all coefficients. and we consider the formula as selected during the experiment.
/// The indicator's delay is much less than the SMA and is comparable to the EMA.
/// But, unlike EMA, the indicator does not respond to HF.
/// </summary>      
public class SuperSmoother : AbstractIndicator
{
    AbstractIndicator _source;
    int _period;
    double _b0, _b1, _a1, _a2;
      
    public SuperSmoother(AbstractIndicator source = null, int period = 7)
    {            
        _period = period;
        _source = source ?? new Close();

        double a = Math.Exp(-Math.Sqrt(2) * Math.PI / _period);
        _a1 = -2d * a * Math.Cos(Math.Sqrt(2) * Math.PI / _period);
        _a2 = a * a;
        _b0 = (1 + _a1 + _a2) / 2d; // SMA(2) дает доп. задержку на 0.5 бара, но фильтрует на частоте Найквиста в 3.5 раза лучше (с -20dB до -70dB)
        _b1 = _b0; // Т.к. берем SMA(2), то весовой коэфф. для текущего и прошлого значения будет одинаковым

        RegisterIndicator(_source);

        Id = $"SuperSmoother:{_period}({_source.Id})";
        IsSeparateWindow = true;
        WarmupBars = _period;
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var currentValue = _source.GetValue(bars.CurrentBar);
        if (!currentValue.HasValue) return null;
        if (bars.Count < 3 || GetValue(bars[1]) == null || GetValue(bars[2]) == null) return currentValue;

        var previousValue = GetValue(bars[1]);
        return _b0 * currentValue + _b1 * _source.GetValue(bars[1]) - _a1 * previousValue - _a2 * GetValue(bars[2]);
    }
}