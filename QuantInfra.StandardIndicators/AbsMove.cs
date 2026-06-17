using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

public class AbsMove : AbstractIndicator
{
    private readonly double _initialValue;
    private readonly AbstractIndicator _source;

    public AbsMove(double initialValue, AbstractIndicator? source = null)
    {
        _initialValue = initialValue;
        _source = source ?? new Close();
        RegisterIndicator(_source);
        Id = $"AbsMove:{initialValue}({_source.Id})";
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        if (bars.CurrentBar == null) return null;

        var val = _source.GetValue(bars.CurrentBar);
        return val.HasValue ? Math.Round(Math.Abs(val.Value / _initialValue - 1), 6) : null;
    }
}