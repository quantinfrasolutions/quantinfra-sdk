using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

public class VWAP : AbstractIndicator
{
    private readonly AbstractIndicator _source;
    private readonly int _period;

    public VWAP(AbstractIndicator source = null, int period = 10)
    {
        _period = period;
        _source = source ?? new Close();
        RegisterIndicator(_source, period);
    }
    
    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var weighted = bars.Take(_period).Sum(b => _source.GetValue(b) * b.Volume);
        var volume = bars.Take(_period).Sum(b => b.Volume);

        return volume != 0 ? weighted / volume : null;
    }
}