using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>   
    /// Lower boilinger band is Sma + coef * Standart Deviation for period. 
    /// </summary>
    public class BollingerUpper : AbstractIndicator
    {
        SimpleMovingAverage _sma;
        StDevPopulation _stdevP;
        int _period;
        double _coef;

        public BollingerUpper(AbstractIndicator source = null, int period = 9, double coef = 1.5)
        {
            source = source ?? new Close();
            _period = period;
            _coef = coef;

            _stdevP = new StDevPopulation(source, _period);
            _sma = new SimpleMovingAverage(source, _period);

            RegisterIndicator(_stdevP);
            RegisterIndicator(_sma);

            Id = $"BollingerUpper:{_period}:{_coef}({source.Id})";
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            return (double)(_sma.GetValue(bars[0]) + _stdevP.GetValue(bars[0]) * _coef);
        }
    }
}
