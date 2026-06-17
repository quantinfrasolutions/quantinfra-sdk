using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>   
    /// Instead of double sequential smoothing of the result through SMA as in the original by George Lane
    /// John Ehlers smooths the original prices through Roofing (removes trends),
    /// and then smoothes the result through SuperSmoother (removes noise)
    /// John Ehlers considers all periods less than 10 unsuitable for trading due to aliasing and indicator delays
    /// Due to the fact that Roofing can take negative values,
    /// the indicator is not normalized along the boundaries (may be more than 100.
    /// Similar to the slow Stochastic after the second SMA smoothing.
    /// </summary>
    public class MesaStochastic : AbstractIndicator
    {
        AbstractIndicator _roofing, _superSmoother;

        public MesaStochastic(AbstractIndicator source = null, int period = 7)
        {
            source = source ?? new Close();
            _roofing = new RoofingStochastic(source, period);
            _superSmoother = new SuperSmoother(_roofing, 10);

            RegisterIndicator(_superSmoother);

            Id = $"MesaStochastic:{period}({source.Id})";
            IsSeparateWindow = true;
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            return _superSmoother.GetValue(bars.CurrentBar) * 100d;
        }
    }
}