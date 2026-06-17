using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>   
    /// Exponential moving average is a type of moving average (MA) that places
    /// a greater weight and significance on the most recent data points.
    /// </summary>
    public class EMA : AbstractIndicator
    {
        int _period;
        AbstractIndicator _underlyingIndicator;

        public EMA(AbstractIndicator ind = null, int period = 9)
        {
            _period = period;
            _underlyingIndicator = ind ?? new Close();
            RegisterIndicator(_underlyingIndicator);

            Id = $"EMA:{_period}({_underlyingIndicator.Id})";
            WarmupBars = _period;
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            var currentValue = _underlyingIndicator.GetValue(bars.CurrentBar);
            if (!currentValue.HasValue) return null;

            var previousValue = bars.Count > 1 ? GetValue(bars[1]) : null;
            if (!previousValue.HasValue) return currentValue; // if this is the first bar when EMA starts calculating

            double multiplier = 2.0 / (_period + 1.0);
            return previousValue + (currentValue - previousValue) * multiplier;
        }
    }
}
