using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>   
    /// The directional movement index identifies in which direction the price
    /// is moving. The indicator does this by comparing prior highs and lows and
    /// drawing two lines: a positive directional movement line (+DI)
    /// and a negative directional movement line (-DI).
    /// When +DI is above -DI, there is more upward pressure than downward pressure in the price and vice versa.
    /// </summary>
    public class DX : AbstractIndicator
    {
        DirectionalIndex _positiveDI;
        DirectionalIndex _negativeDI;

        public DX(AbstractIndicator high = null, AbstractIndicator low = null,
        AbstractIndicator close = null, int period = 7)
        {
            high = high ?? new High();
            low = low ?? new Low();
            close = close ?? new Close();
            _positiveDI = new DirectionalIndex(high, low, close, period, true);
            _negativeDI = new DirectionalIndex(high, low, close, period, false);
            RegisterIndicator(_positiveDI);
            RegisterIndicator(_negativeDI);
            Id = $"DX:{period}({high.Id},{low.Id},{close.Id})";
            IsSeparateWindow = true;
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            return ((_positiveDI.GetValue(bars[0]) - _negativeDI.GetValue(bars[0])) /
                   (_positiveDI.GetValue(bars[0]) + _negativeDI.GetValue(bars[0]))) * 100;
        }
    }
}