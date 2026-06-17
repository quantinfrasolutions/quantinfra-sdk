using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Lowest value of underlying indicator for period.
/// </summary>
public class Lowest : AbstractIndicator
{
    int _period;
    AbstractIndicator _underlyingIndicator;

    public Lowest(AbstractIndicator ind, int period = 10)
    {
        _underlyingIndicator = ind ?? new Low();
        _period = period;
        RegisterIndicator(_underlyingIndicator, _period - 1);

        Id = $"Lowest:{_period}({_underlyingIndicator.Id})";
    }

    protected override double? Calculate(IBarStorage bars, double? price = null) =>
        bars.Take(_period).Select(b => _underlyingIndicator.GetValue(b)).Min();
}