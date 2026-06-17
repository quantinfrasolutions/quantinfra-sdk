using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para> 
    /// The formula calculates the difference between 2 indicators
    /// </summary>
    public class Difference : AbstractIndicator
    {
        AbstractIndicator _underlyingIndicator;
        AbstractIndicator _substractIndicator;

        public Difference(AbstractIndicator underlyingIndicator, AbstractIndicator substractIndicator)
        {
            _underlyingIndicator = underlyingIndicator;
            _substractIndicator = substractIndicator;
            RegisterIndicator(_substractIndicator, 1);
            RegisterIndicator(_underlyingIndicator, 1);

            Id = $"Difference:{underlyingIndicator.Id}({substractIndicator.Id})";
            IsSeparateWindow = true;
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
            => _underlyingIndicator.GetValue(bars[0]) - _substractIndicator.GetValue(bars[0]);
        
    }
}

