using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    ///  The Hull Moving Average (HMA) is an extremely fast and smooth moving average.
    ///  In fact, the HMA almost eliminates lag altogether and manages
    ///  to improve smoothing at the same time.
    /// </summary>
    public class HMA : AbstractIndicator
    {
        WMA _wma;
        HMASource _source;

        public HMA(AbstractIndicator ind, int period = 9)
        {
            ind = ind ?? new Close();
            _source = new HMASource(ind, period);
            _wma = new WMA(_source, Convert.ToInt32(Math.Ceiling(Math.Sqrt(period))));
            RegisterIndicator(_wma);

            Id = $"HMA:{period}({ind.Id})";
        }

        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            return _wma.GetValue(bars[0]);
        }
    }
}