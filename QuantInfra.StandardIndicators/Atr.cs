using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>  
    /// The true range indicator is taken as the greatest of the following:
    /// current high less the current low; the absolute value of the current
    /// high less the previous close; and the absolute value of the current low
    /// less the previous close.
    /// The ATR is then a moving average, generally using 14 days, of the true ranges.
    /// </summary>
    public class ATR : AbstractIndicator
    {
        int _period;
        AbstractIndicator _trueRange;


        public ATR(AbstractIndicator high = null, AbstractIndicator low = null,
                                AbstractIndicator close = null, int period = 14)
        {
            _period = period;
            high = high ?? new High();
            low = low ?? new Low();
            close =  close?? new Close();
            _trueRange = new TrueRange(high, low, close);

            RegisterIndicator(_trueRange, _period - 1);

            Id = $"Atr:{_period}({high.Id}, {low.Id}, {close.Id})";
            IsSeparateWindow = true;
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            return bars.Take(_period).Sum(_trueRange.GetValue) / _period;
        }
    }
}