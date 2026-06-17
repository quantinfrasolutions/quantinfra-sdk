using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>  
    /// DownCandlesDifference is weighted cumulative value of all falling candles for n periods.
    /// </summary>
    public class DownCandlesDifference : AbstractIndicator
    {
        int _period;
        AbstractIndicator _underlyingIndicator;

        public DownCandlesDifference(AbstractIndicator ind, int period = 14)
        {
            _period = period;
            _underlyingIndicator = ind ?? new Close();
            RegisterIndicator(_underlyingIndicator, period);

            Id = $"DownCandlesDifference:{_period}({_underlyingIndicator.Id})";
            IsSeparateWindow = true;
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            double? down = 0.0, dif;
            if (!GetValue(bars[1]).HasValue)
            {
                for (int i = 0; i < _period; i++)
                {
                    if (_underlyingIndicator.GetValue(bars[i]) < _underlyingIndicator.GetValue(bars[i + 1]))
                    {
                        dif = _underlyingIndicator.GetValue(bars[i + 1]) - _underlyingIndicator.GetValue(bars[i]);
                        down += dif;
                    }
                }
                down /= _period;
            }
            else
            {
                if (_underlyingIndicator.GetValue(bars[0]) < _underlyingIndicator.GetValue(bars[1]))
                {
                    down = _underlyingIndicator.GetValue(bars[1]) - _underlyingIndicator.GetValue(bars[0]);
                }
                down = (GetValue(bars[1]) * (_period - 1) + down) / _period;
            }
            return down;
        }
    }
}
