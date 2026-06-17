using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para> 
    /// Keltner Channels are volatility-based bands that are placed on either <summary>
    /// Keltner Channels are volatility-based bands that are placed on either
    /// side of an asset's price and can aid in determining the direction of a trend.
    /// KeltnerDown is calculated as EMA - 2*ATR
    /// </summary>
    public class KeltnerDown : AbstractIndicator
    {
        ATR _atr;
        EMA _ema;

        public KeltnerDown(AbstractIndicator high = null, AbstractIndicator low = null,
                   AbstractIndicator close = null, AbstractIndicator source = null, int period = 9)
        {
            high = high ?? new High();
            low = low ?? new Low();
            close = close ?? new Close();
            source = source ?? new Close();

            _atr = new ATR(high, low, close, period);
            _ema = new EMA(source, period);

            RegisterIndicator(_atr);
            RegisterIndicator(_ema);

            Id = $"KeltnerDown:{period}({high.Id},{low.Id},{close.Id},{source.Id})";
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            return _ema.GetValue(bars[0]) - 2 * _atr.GetValue(bars[0]);
        }
    }
}
