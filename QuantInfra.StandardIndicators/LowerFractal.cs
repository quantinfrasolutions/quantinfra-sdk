using System;
using System.Collections.Generic;
using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Search the Fractal with the following algorithm: if the index of min value of underlying indicator
/// is equal to the rounded (period / 2), the LowerFractal found.
/// </summary>
public class LowerFractal : AbstractIndicator
{
    int _period;
    AbstractIndicator _underlyingIndicator;
    int _middleIndex;

    public LowerFractal(AbstractIndicator ind, int period = 14)
    {
        _period = period;
        _underlyingIndicator = ind ?? new Close();
        _middleIndex = (int)Math.Ceiling((double)period / 2);
        RegisterIndicator(_underlyingIndicator, _period - 1);

        Id = $"LowerFractal:{_period}({_underlyingIndicator.Id})";
        IsSeparateWindow = true;
        WarmupBars = _period - 1;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        if (bars.Count < 2 || !GetValue(bars[1]).HasValue) return _underlyingIndicator.GetValue(bars[0]);

        double res;
        List<double?> tmp = bars
            .Take(_period)
            .Select(_underlyingIndicator.GetValue)
            .ToList();

        int maxValueIndex = tmp.IndexOf(tmp.Min());

        res = (double)(maxValueIndex == _middleIndex ? tmp.Min() : GetValue(bars[1]));
        return res;
    }
}