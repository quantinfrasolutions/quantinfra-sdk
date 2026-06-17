using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>   
    /// Underlying indicator multiplied by Volume.
    /// </summary>
    public class VolumeWeighted : AbstractIndicator
    {
        AbstractIndicator _underlyingIndicator;
        AbstractIndicator _volume;

        public VolumeWeighted(AbstractIndicator ind, AbstractIndicator volume)
        {
            
            _underlyingIndicator = ind ?? new Close();
            _volume = volume ?? new Volume();

            RegisterIndicator(_underlyingIndicator);
            RegisterIndicator(_volume);

            Id = $"VolumeWeighted({_underlyingIndicator.Id}, {_volume.Id})";
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            return _underlyingIndicator.GetValue(bars.CurrentBar) * _volume.GetValue(bars.CurrentBar);
        }
    }
}