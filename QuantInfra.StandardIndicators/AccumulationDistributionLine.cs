using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// The accumulation/distribution indicator (A/D) is a cumulative indicator
/// that uses volume and price to assess whether a stock is being accumulated
/// or distributed. The A/D measure seeks to identify divergences between
/// the stock price and the volume flow. This provides insight into how
/// strong a trend is. If the price is rising but the indicator is falling,
/// then it suggests that buying or accumulation volume may not be enough
/// to support the price rise and a price decline could be forthcoming.
/// </summary>
public class AccumulationDistributionLine : AbstractIndicator
{
    AbstractIndicator _high;
    AbstractIndicator _low;
    AbstractIndicator _close;


    public AccumulationDistributionLine(AbstractIndicator high = null, AbstractIndicator low = null,
        AbstractIndicator close = null)
    {
        _high = high?? new High();
        _close = close ?? new Close();
        _low = low ?? new Low();

        RegisterIndicator(_high);
        RegisterIndicator(_close);
        RegisterIndicator(_low);

        Id = $"AccumulationDistributionLine({_high.Id},{_close.Id},{_low.Id})";
        WarmupBars = 1;
        IsSeparateWindow = true;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        var currentValue = (((_close.GetValue(bars[0]) - _low.GetValue(bars[0])) -
                             (_high.GetValue(bars[0]) - _close.GetValue(bars[0])))/ (_high.GetValue(bars[0]) - _low.GetValue(bars[0]))) * bars[0].Volume;
        if (bars.Count < 2 || !GetValue(bars[1]).HasValue) return currentValue;
        return currentValue + GetValue(bars[1]);
    }
}