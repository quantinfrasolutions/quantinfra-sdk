using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para> 
    /// On-balance volume provides a running total of an asset's trading volume
    /// and indicates whether this volume is flowing in or out
    /// of a given security or currency pair.
    /// The OBV is a cumulative total of volume (positive and negative).
    /// </summary>
    public class OBV : AbstractIndicator
    {
        AbstractIndicator _source;

        public OBV(AbstractIndicator close = null)
        {
            _source = close ?? new Close();
            RegisterIndicator(_source, 1);

            Id = $"OBV({_source.Id})";
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            if (GetValue(bars[1]) == null) return _source.GetValue(bars[0]) > _source.GetValue(bars[1])? -bars[0].Volume : bars[0].Volume;
            double? result = 0.0;
            if (_source.GetValue(bars[0])> _source.GetValue(bars[1]))
            {
                result = GetValue(bars[1]) + bars[0].Volume;
            }
            else if(_source.GetValue(bars[0]) < _source.GetValue(bars[1]))
            {
                result = GetValue(bars[1]) - bars[0].Volume;
            }
            else if (_source.GetValue(bars[0]) == _source.GetValue(bars[1]))
            {
                result = GetValue(bars[1]);
            }
            return result;
        }
    }
}
