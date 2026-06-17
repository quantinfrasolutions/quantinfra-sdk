using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para> 
/// Logarithm of an underlying indicator
/// </summary>
public class Log : AbstractIndicator
{
    double _base;
    AbstractIndicator _underlyingIndicator;

    public Log(AbstractIndicator ind, double logBase = Math.E)
    {
        _base = logBase;
        _underlyingIndicator = ind ?? new Close();
        RegisterIndicator(_underlyingIndicator);

        Id = $"Log:{_base}({_underlyingIndicator.Id})";
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return  Math.Log((double)_underlyingIndicator.GetValue(bars.CurrentBar), _base);
    }
}