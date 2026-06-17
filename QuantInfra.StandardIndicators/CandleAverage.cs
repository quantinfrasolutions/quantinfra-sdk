using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>   
    /// Average value of Open, High, Low, CLose for 1 candle. 
    /// </summary>
    public class CandleAverage : AbstractIndicator
    {
        AbstractIndicator _open, _high, _low, _close;

        public CandleAverage(AbstractIndicator open = null,
                             AbstractIndicator high = null,
                             AbstractIndicator low = null,
                             AbstractIndicator close = null)
        {
            _open = open ?? new Open();
            _high = high ?? new High();
            _low = low ?? new Low();
            _close = close ?? new Close();

            RegisterIndicator(_open);
            RegisterIndicator(_high);
            RegisterIndicator(_close);
            RegisterIndicator(_low);

            Id = $"CandleAverage({_open.Id},{_high.Id},{_low.Id},{_close.Id})";
            IsSeparateWindow = true;
        }


        protected override double? Calculate(IBarStorage bars, double? price = null) =>
            (_high.GetValue(bars[0]) + _low.GetValue(bars[0])
            + _close.GetValue(bars[0]) + _open.GetValue(bars[0])) / 4;
    }
}