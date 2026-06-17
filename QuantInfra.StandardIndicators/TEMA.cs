using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>   
    /// TEMA is calculated as follows:
    /// 3*EMA - 3*doubleEMA - tripleEMA
    /// </summary>
    public class TEMA : AbstractIndicator
    {
        AbstractIndicator _underlyingIndicator;
        EMA _ema;
        EMA _doubleEMA;
        EMA _tripleEMA;
        int _period;

        public TEMA(AbstractIndicator ind, int period = 9)
        {
            _underlyingIndicator = ind ?? new Close();
            _period = period;
            _ema = new EMA(_underlyingIndicator, _period);
            _doubleEMA = new EMA(_ema, _period);
            _tripleEMA = new EMA(_doubleEMA, _period);
            RegisterIndicator(_tripleEMA);

            Id = $"TEMA:{_period}({_underlyingIndicator.Id})";
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            return 3 * _ema.GetValue(bars[0]) - 3 * _doubleEMA.GetValue(bars[0]) + _tripleEMA.GetValue(bars[0]);
        }
    }
}
