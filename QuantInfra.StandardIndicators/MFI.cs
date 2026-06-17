using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>  
    /// The Money Flow Index (MFI) is a technical oscillator that uses price
    /// and volume data for identifying overbought or oversold signals in an asset.
    /// It can also be used to spot divergences which warn of a trend change in price.
    /// Unlike conventional oscillators such as the Relative Strength Index(RSI),
    /// the Money Flow Index incorporates both price and volume data, as opposed to
    /// just price.For this reason, some analysts call MFI the volume-weighted RSI.
    /// </summary>
    public class MFI : AbstractIndicator
    {
        NegativeMF _negativeMf;
        PositiveMF _positiveMf;

        public MFI(AbstractIndicator high = null, AbstractIndicator low = null,
                   AbstractIndicator close = null, int period = 9)
        {
            high = high ?? new High();
            low = low ?? new Low();
            close = close ?? new Close();

            _negativeMf = new NegativeMF(high, low, close, period);
            _positiveMf = new PositiveMF(high, low, close, period);

            RegisterIndicator(_positiveMf);
            RegisterIndicator(_negativeMf);

            Id = $"MFI:{period}({high.Id}, {low.Id}, {close.Id})";
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            var mfRatio  = _positiveMf.GetValue(bars[0]) / _negativeMf.GetValue(bars[0]);
            return 100 - (100/(1+mfRatio));
        }
    }
}
