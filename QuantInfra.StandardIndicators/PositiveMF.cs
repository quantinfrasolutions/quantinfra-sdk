using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// PositiveMF is Row Money Flow for only raising candles.
/// multiplied by Volume.
/// </summary>
public class PositiveMF : AbstractIndicator
{
    RawMoneyFlow _rawMf;
    int _period;
    AbstractIndicator _source;

    public PositiveMF(AbstractIndicator high = null, AbstractIndicator low = null,
        AbstractIndicator close = null, int period  = 9)
    {
        high = high ?? new High();
        low = low ?? new Low();            
        _source  = close ?? new Close();
        _rawMf = new RawMoneyFlow(high, low, _source);
        _period = period;

        RegisterIndicator(_source, _period);
        RegisterIndicator(_rawMf, _period);

        Id = $"PositiveMF:{period}({high.Id},{low.Id},{close.Id})";
        IsSeparateWindow = true;
    }

    protected override double? Calculate(IBarStorage bars, double? price = null) =>
        Enumerable
            .Range(0, _period)
            .Where(i => _source.GetValue(bars[i]) > _source.GetValue(bars[i + 1]))
            .Sum(i => _rawMf.GetValue(bars[i]));
}