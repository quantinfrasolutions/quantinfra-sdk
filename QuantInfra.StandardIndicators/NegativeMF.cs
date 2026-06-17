using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// NegativeMF is Row Money Flow for only falling candles.
/// multiplied by Volume.
/// </summary>
public class NegativeMF : AbstractIndicator
{
    RawMoneyFlow _rawMf;
    int _period;
    AbstractIndicator _source;

    public NegativeMF(AbstractIndicator high = null, AbstractIndicator low = null,
        AbstractIndicator close = null, int period = 9)
    {
        _rawMf = new RawMoneyFlow(high, low, close);
        _period = period;
        _source = close ?? new Close();

        RegisterIndicator(_rawMf, _period);

        Id = $"NegativeMF:{_rawMf.Id}";
        IsSeparateWindow = true;
    }

    protected override double? Calculate(IBarStorage bars, double? price = null) =>
        Enumerable
            .Range(0, _period)
            .Where(i => _source.GetValue(bars[i]) < _source.GetValue(bars[i + 1]))
            .Sum(i => _rawMf.GetValue(bars[i]));
}