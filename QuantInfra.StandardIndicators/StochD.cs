using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// StochD is averaged StochK stochastic indicator.
/// </summary>
public class StochD : AbstractIndicator
{
    StochK _stochK;
    SimpleMovingAverage _sma;

    public StochD(int numberOfPeriods = 14, int smoothingPeriods = 3)
    {
        _stochK = new StochK(numberOfPeriods);
        _sma = new SimpleMovingAverage(_stochK, smoothingPeriods);

        RegisterIndicator(_sma);

        Id = $"StochD:{numberOfPeriods}:{smoothingPeriods}";
        IsSeparateWindow = true;
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return _sma.GetValue(bars[0]);
    }
}