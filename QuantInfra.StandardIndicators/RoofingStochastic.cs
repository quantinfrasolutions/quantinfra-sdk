using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Implementation of stohastic based on Digital Filters course.
/// Similar to Stochastic after averaging with SMA.
/// </summary>
public class RoofingStochastic : AbstractIndicator
{
    AbstractIndicator _roofing, _maxRoothing, _minRoothing;

    public RoofingStochastic(AbstractIndicator source = null, int period = 7)
    {
        source = source ?? new Close();
        _roofing = new Roofing(source, 10,48);
        _maxRoothing = new Highest(_roofing, period);
        _minRoothing = new Lowest(_roofing, period);

        RegisterIndicator(_roofing);
        RegisterIndicator(_maxRoothing);
        RegisterIndicator(_minRoothing);

        Id = $"RoofingStochastic:{period}({source.Id})";
        IsSeparateWindow = true;
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return (_roofing.GetValue(bars.CurrentBar) - _minRoothing.GetValue(bars.CurrentBar))/
               (_maxRoothing.GetValue(bars.CurrentBar) - _minRoothing.GetValue(bars.CurrentBar));
    }
}