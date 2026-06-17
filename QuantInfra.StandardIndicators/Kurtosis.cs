using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para> 
/// Kurtosis describes the "fatness" of the tails found in probability distributions.
/// There are three kurtosis categories—mesokurtic(normal), platykurtic(less than normal),
/// and leptokurtic(more than normal).
/// Kurtosis risk is a measurement of how often an investment's price moves dramatically.
/// A curve's kurtosis characteristic tells you how much kurtosis risk the investment you're evaluating has.
/// TODO: try this to filter strategies
/// </summary>
public class Kurtosis : AbstractIndicator
{
    int _period;
    AbstractIndicator _source;


    public Kurtosis(AbstractIndicator source = null, int period = 9)
    {
        _period = period;
        _source = source ?? new Close();

        RegisterIndicator(_source, _period);

        Id = $"Kurtosis:{_period}({_source.Id})";
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        double? sourceCum = 0, sourceCumSqrt = 0, sourceCumThrd = 0, sourceCumSqrtSqrt = 0;
        for (int i = 0; i < _period; i++)
        {
            sourceCum += _source.GetValue(bars[i]);
            sourceCumSqrt += Math.Pow((double)_source.GetValue(bars[i]), 2);
            sourceCumThrd += Math.Pow((double)_source.GetValue(bars[i]), 2) * _source.GetValue(bars[i]);
            sourceCumSqrtSqrt += Math.Pow(Math.Pow((double)_source.GetValue(bars[i]), 2),2);
        }
        double? averageSrc = sourceCum/_period;
        double? avreageDiffSqrt = (sourceCumSqrt- sourceCum* averageSrc)/(_period-1);
        if (avreageDiffSqrt < 0) return 0;
        double? multiplier = _period * sourceCumSqrtSqrt - 4 * sourceCumThrd * sourceCum +
            6 * sourceCumSqrt * sourceCum * averageSrc - 3 * Math.Pow((double)(sourceCum * averageSrc), 2);
        double? secondMultiplier = (_period + 1) /
                                   ((_period - 1) * (_period - 2) * (_period - 3) * Math.Pow((double)avreageDiffSqrt, 2))
                                   - 3 * Math.Pow(_period - 1, 2) / ((_period - 2) * (_period - 3));
        return multiplier * secondMultiplier;
    }
}