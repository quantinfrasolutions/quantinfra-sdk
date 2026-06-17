using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Z-score is a statistical measurement that describes a value's relationship to the mean of a group of values.
/// Z-score is measured in terms of standard deviations from the mean.
/// If a Z-score is 0, it indicates that the data point's score is identical to the mean score.
/// </summary>
public class ArbitrageZScore : AbstractIndicator
{
    private readonly StDevPopulation _stdev;
    private readonly AbstractIndicator _avg;
    private readonly AbstractIndicator _basePrice;
    
    public ArbitrageZScore(AbstractIndicator basePrice, AbstractIndicator avg, int period = 9)
    {
        _basePrice = basePrice;
        _stdev = new StDevPopulation(basePrice, period);
        _avg = avg;

        RegisterIndicator(_stdev, period);
        RegisterIndicator(_avg, period);
        RegisterIndicator(_basePrice);

        Id = $"ArbitrageZScore:{period}({_stdev.Id},{_avg.Id})";
        IsSeparateWindow = true;
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return (_basePrice.GetValue(bars[0]) - _avg.GetValue(bars[0])) / _stdev.GetValue(bars[0]);
    }
}