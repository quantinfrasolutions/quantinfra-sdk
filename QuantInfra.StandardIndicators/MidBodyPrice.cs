using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para> 
    /// MidBodyPrice is average of open and close.
    /// </summary>
    public class MidBodyPrice : AbstractIndicator
    {
        AbstractIndicator _open, _close;

        public MidBodyPrice(AbstractIndicator open = null,
                           AbstractIndicator close = null)
        {
            _open = open ?? new Open();
            _close = close ?? new Close();
            RegisterIndicator(_open);
            RegisterIndicator(_close);

            Id = $"MidBodyPrice({_open.Id},{_close.Id})";
            IsSeparateWindow = true;
        }

        protected override double? Calculate(IBarStorage bars, double? price = null)
            => (_open.GetValue(bars[0]) + _close.GetValue(bars[0])) / 2;
    }
}