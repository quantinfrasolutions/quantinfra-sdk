using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>  
    /// The Price Rate of Change (ROC) is an indicator that measures
    /// the percentage change in price between the current price
    /// and the price a certain number of periods ago.
    /// </summary>
    public class ROC : AbstractIndicator
    {
        int _period;
        bool _toPercent;
        AbstractIndicator _underlyingIndicator;

        public ROC(AbstractIndicator ind, int period = 9, bool toPercent = true)
        {
            _period = period;
            _toPercent = toPercent;
            _underlyingIndicator = ind ?? new Close();
            RegisterIndicator(_underlyingIndicator, _period);

            Id = $"ROC:{_period}({_underlyingIndicator.Id})";
            IsSeparateWindow = true;
        }

        protected override double? Calculate(IBarStorage bars, double? price = null) =>
            _toPercent
                ? (_underlyingIndicator.GetValue(bars[0]) - _underlyingIndicator.GetValue(bars[_period]))
                  / _underlyingIndicator.GetValue(bars[_period]) * 100
                : (_underlyingIndicator.GetValue(bars[0]) - _underlyingIndicator.GetValue(bars[_period]))
                  / _underlyingIndicator.GetValue(bars[_period]);
    }
}
