using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// The Relative Strength Index (RSI) is a momentum oscillator that measures
/// the speed and change of price movements. The RSI oscillates between zero and 100. 
/// </summary>
public class RSI : AbstractIndicator
{
    UpCandlesDifference _up;
    DownCandlesDifference _down;


    public RSI(AbstractIndicator ind, int period = 14)
    {
        ind = ind ?? new Close();
        _up = new UpCandlesDifference(ind, period);
        _down = new DownCandlesDifference(ind, period);
        RegisterIndicator(_up);
        RegisterIndicator(_down);

        Id = $"RSI:{period}({ind.Id})";
        IsSeparateWindow = true;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        double coef = (double)((!(_down.GetValue(bars[0]) > 0.0)) ? 1.0 : (_up.GetValue(bars[0]) / _down.GetValue(bars[0])));
        return 100.0 - 100.0 / (1.0 + coef);
    }
}