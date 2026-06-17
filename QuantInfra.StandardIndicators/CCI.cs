using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para> 
    ///  The Commodity Channel Index​ (CCI) is a momentum-based oscillator,
    ///  this technical indicator assesses price trend direction and strength,
    ///  allowing traders to determine if they want to enter or exit a trade,
    ///  refrain from taking a trade, or add to an existing position.
    ///  Calculated as (Typical price - average)(inverse factor * mean deviation)
    /// </summary>
    public class CCI : AbstractIndicator
    {
        double _inverseFactor;
        int _period;

        HLC3 _hlc3;
        SimpleMovingAverage _sma;
        MeanAbsoluteDeviation _mad;

        public CCI(
            double inverseFactor = 0.015,
            int period = 20
        )
        {
            _inverseFactor = inverseFactor;
            _period = period;
            _hlc3 = new HLC3(new High() ,new Low(),new Close());
            _sma = new SimpleMovingAverage(_hlc3, period);
            _mad = new MeanAbsoluteDeviation(_hlc3, period);

            RegisterIndicator(_hlc3);
            RegisterIndicator(_sma);
            RegisterIndicator(_mad);

            Id = $"CCI:{_inverseFactor}:{_period}";
            IsSeparateWindow = true;
            WarmupBars = _period;
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            return (_hlc3.GetValue(bars[0]) - _sma.GetValue(bars[0])) / (_inverseFactor * _mad.GetValue(bars[0]));
        }
    }
}
