using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

public class CumulativeVwap : AbstractIndicator
{
    private readonly AbstractIndicator _source;

    public CumulativeVwap(AbstractIndicator source = null)
    {
        _source = source ?? new Close();
        RegisterIndicator(_source);
    }
    
    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var weighted = bars.Sum(b => _source.GetValue(b) * b.Volume);
        var volume = bars.Sum(b => b.Volume);

        return volume != 0 ? weighted / volume : null;
    }
}