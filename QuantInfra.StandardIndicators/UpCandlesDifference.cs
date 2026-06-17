using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>  
/// UpCandlesDifference is weighted cumulative value of all raising candles for n periods.
/// </summary>
public class UpCandlesDifference : AbstractIndicator
{
    int _period;
    AbstractIndicator _underlyingIndicator;

    public UpCandlesDifference(AbstractIndicator ind, int period = 14)
    {
        _period = period;
        _underlyingIndicator = ind ?? new Close();
        RegisterIndicator(_underlyingIndicator, _period);

        Id = $"UpCandlesDifference:{_period}:({_underlyingIndicator.Id})";
        IsSeparateWindow = true;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        double? up = 0.0, dif;
        if (!GetValue(bars[1]).HasValue)
        {
            for (int i = 0; i < _period; i++)
            {
                if (_underlyingIndicator.GetValue(bars[i]) > _underlyingIndicator.GetValue(bars[i + 1]))
                {
                    dif = _underlyingIndicator.GetValue(bars[i]) - _underlyingIndicator.GetValue(bars[i + 1]);
                    up += dif;
                }
            }
            up /= _period;
        }
        else
        {
            if (_underlyingIndicator.GetValue(bars[0]) > _underlyingIndicator.GetValue(bars[1]))
            {
                up = _underlyingIndicator.GetValue(bars[0]) - _underlyingIndicator.GetValue(bars[1]);
            }

            up = (GetValue(bars[1]) * (_period - 1) + up) / _period;
        }
        return up;
    }
}