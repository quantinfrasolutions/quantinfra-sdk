using System;
using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

public class Volatility : AbstractIndicator
{
    private readonly AbstractIndicator _source;
    private readonly int _period;

    public Volatility(AbstractIndicator? source = null, int period = 15)
    {
        _source = source ?? new Close();
        _period = period;
        
        RegisterIndicator(_source, _period + 1);

        Id = $"Volatility:{_period}({_source.Id})";
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var values = bars.Take(_period + 1).Select(b => _source.GetValue(b)).ToArray();
        var returns = Enumerable.Range(0, values.Length - 1).Select(i => values[i + 1] / values[i] - 1).ToArray();
        var avg = returns.Average();
        return avg != null
            ? Math.Sqrt(returns.Average(v=> Math.Pow(v!.Value - avg.Value, 2)))
            : null;
    }
}