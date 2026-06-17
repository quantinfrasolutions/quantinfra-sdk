using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>   
    /// Highest value of underlying indicator for period.
    /// </summary>
    public class Highest : AbstractIndicator
    {
        int _period;
        AbstractIndicator _underlyingIndicator;

        public Highest(AbstractIndicator ind, int period = 10)
        {
            _underlyingIndicator = ind ?? new Close();
            _period = period;
            RegisterIndicator(_underlyingIndicator, _period - 1);

            Id = $"Highest:{_period}({_underlyingIndicator.Id})";
        }
        

        protected override double? Calculate(IBarStorage bars, double? price = null) =>
            bars.Take(_period).Select(_underlyingIndicator.GetValue).Max();
    }
}