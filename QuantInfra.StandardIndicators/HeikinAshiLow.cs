using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>  
    /// Heikin-Ashi, also sometimes spelled Heiken-Ashi, means "average bar" in Japanese.
    /// Low is calculated as the min of open,close and low.
    /// </summary>
    public class HeikinAshiLow : AbstractIndicator
    {
        AbstractIndicator _close, _open;
        public HeikinAshiLow()
        {
            _close = new HeikinAshiClose();
            _open = new HeikinAshiOpen();
            RegisterIndicator(_close);
            RegisterIndicator(_open);

            Id = $"HeikinAshiLow";
        }

        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            return new[] { bars[0].Low, _close.GetValue(bars[0]), _open.GetValue(bars[0]) }.Min();
        }

    }
}
