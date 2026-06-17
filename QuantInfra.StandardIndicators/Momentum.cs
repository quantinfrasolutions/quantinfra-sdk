using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para> 
    /// Momentum is a derivative of underlying.
    /// </summary>
    public class Momentum : AbstractIndicator
    {
        int _period;
        AbstractIndicator _underlyingIndicator;

        public Momentum(AbstractIndicator ind = null, int period = 9)
        {
            _period = period;
            _underlyingIndicator = ind ?? new Close();
            RegisterIndicator(_underlyingIndicator, _period);

            Id = $"Momentum:{_period}({_underlyingIndicator.Id})";
            IsSeparateWindow = true;
        }

        protected override double? Calculate(IBarStorage bars, double? price = null)
            => _underlyingIndicator.GetValue(bars[0]) - _underlyingIndicator.GetValue(bars[_period]);
    }
}