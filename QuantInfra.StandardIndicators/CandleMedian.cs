using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>   
    /// Median value of High and Low  for 1 candle. 
    /// </summary>
    public class CandleMedian : AbstractIndicator
    {

        AbstractIndicator _high, _low;

        public CandleMedian( AbstractIndicator high = null,
                         AbstractIndicator low = null)
        {
            _high = high ?? new High();
            _low = low ?? new Low();
            RegisterIndicator(_high);
            RegisterIndicator(_low);
            Id = $"CandleMedian({_high.Id},{_low.Id})";
            IsSeparateWindow = true;
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            return (_low.GetValue(bars[0]) + _high.GetValue(bars[0])) / 2;
        }
            
    }
}