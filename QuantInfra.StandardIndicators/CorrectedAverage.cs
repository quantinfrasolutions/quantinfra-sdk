using System;
using System.Linq;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// The strengths of the Corrected Average(CA) is that the current value of
/// time series must exceed the current volatility-dependent threshold, so that
/// the filter increases or falls, avoiding false signals if trend is in weak phase
/// </summary>
public class CorrectedAverage : AbstractIndicator
{
    int _period;
    AbstractIndicator _underlyingIndicator;
    StDevPopulation _stdevP;
    public double? CA = 0.0;
    public double? SA = 0.0;
    public double? volThreshold, movingDiff, k, prevCA;

    public CorrectedAverage(AbstractIndicator ind, int period = 9)
    {
        _period = period;
        _underlyingIndicator = ind ?? new Close();
        _stdevP = new StDevPopulation(ind, _period);
        RegisterIndicator(_underlyingIndicator, _period - 1);
        RegisterIndicator(_stdevP);

        Id = $"CorrectedAverage:{_period}:{_underlyingIndicator.Id}";
        IsSeparateWindow = true;
        WarmupBars = 1;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        if (!GetValue(bars[1]).HasValue)
        {
            prevCA = _underlyingIndicator.GetValue(bars[0]);
        } 

        SA = bars.Take(_period).Select(b => _underlyingIndicator.GetValue(b)).Sum() / _period;

        volThreshold = Math.Pow((double)_stdevP.GetValue(bars[0]), 2);
        movingDiff = Math.Pow((double)(prevCA - SA), 2);
        prevCA = CA;

        if (movingDiff < volThreshold || movingDiff == 0 || volThreshold == 0)
        {
            k = 0.0;
        }
        else if (movingDiff > 0 && volThreshold > 0)
        {

            k = 1 - volThreshold / movingDiff;
            CA = prevCA + k * (SA - prevCA);
        }
           
        return CA;
    }
}