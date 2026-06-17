using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para> 
/// To calculate the Chaikin oscillator, subtract n-period EMA of
/// the accumulation-distribution line from a n-period/3  EMA
/// of the accumulation-distribution line.
/// This measures momentum predicted by oscillations around the A/D line.
/// </summary>
public class ChaikinOscilator : AbstractIndicator
{
    EMA _slowEma, _fastEma;

    public ChaikinOscilator(AbstractIndicator high = null, AbstractIndicator low = null,
        AbstractIndicator close = null, int period=10)
    {
        high = high ?? new High();
        low = low ?? new Low();
        close = close ?? new Close();
        var adl  = new AccumulationDistributionLine(high, low, close);
        _slowEma = new EMA(adl, period);
        _fastEma = new EMA(adl, period/3);

        RegisterIndicator(_slowEma);
        RegisterIndicator(_fastEma);

        Id = $"ChaikinOscilator:({high.Id},{low.Id},{close.Id})";
        IsSeparateWindow = true;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return _fastEma.GetValue(bars[0]) - _slowEma.GetValue(bars[0]);
    }
}