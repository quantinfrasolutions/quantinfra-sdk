using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>   
    /// Helper for ADX indicator. Calculated as WilderMA/ATR * 100
    /// </summary>
    public class DirectionalIndex : AbstractIndicator
    {
        ATR _atr;
        WilderMA _wilderMa;
        
        public DirectionalIndex(AbstractIndicator high = null, AbstractIndicator low = null,
                                AbstractIndicator close = null, int period = 7, bool isPositive = true)
        {
            high = high ?? new High();
            low = low ?? new Low();
            close = close ?? new Close();
            _atr = new ATR(high, low, close, period);
            var di = new DirectionalMovement(high, low, isPositive);
            _wilderMa = new WilderMA(di,  period);

            RegisterIndicator(_atr);
            RegisterIndicator(_wilderMa);

            Id = $"DirectionalIndex:{period}:{isPositive}({high.Id},{low.Id},{close.Id})";
            IsSeparateWindow = true;
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            return (_wilderMa.GetValue(bars[0]) / _atr.GetValue(bars[0])) * 100;
        }
    }
}