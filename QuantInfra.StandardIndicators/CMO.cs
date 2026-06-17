using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para> 
    /// The formula calculates the difference between the sum
    /// of recent gains and the sum of recent losses and then divides
    /// the result by the sum of all price movements over the same period.
    /// </summary>
    public class CMO : AbstractIndicator
    {
        int _period;
        AbstractIndicator _underlyingIndicator;

        public CMO(AbstractIndicator ind, int period = 9)
        {
            _period = period;
            _underlyingIndicator = ind ?? new Close();
            RegisterIndicator(_underlyingIndicator, _period);

            Id = $"CMO:{_period}({_underlyingIndicator.Id})";
            IsSeparateWindow = true;
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            double? up = 0.0, down = 0.0, difference;            
            for (var i = 0; i < _period; i++)
            {
                difference = _underlyingIndicator.GetValue(bars[i]) - _underlyingIndicator.GetValue(bars[i + 1]);

                if (difference > 0.0) up += difference;
                if (difference < 0.0) down -= difference;
            }

            return (up - down) / (up + down) * 100.0;
        }
    }
}

