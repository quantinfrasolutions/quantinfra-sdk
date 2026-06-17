using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Helper for ADX indicator. 
/// </summary>
public class DirectionalMovement : AbstractIndicator
{
    bool _isPositive;
    AbstractIndicator _high;
    AbstractIndicator _low;

    public DirectionalMovement(AbstractIndicator high = null, AbstractIndicator low = null,
        bool isPostive = true)
    {
        _isPositive = isPostive;
        _high = high?? new High();
        _low = low?? new Low();

        RegisterIndicator(_high, 1);
        RegisterIndicator(_low, 1);

        Id = $"DirectionalMovement:{_isPositive}({_high.Id},{_low.Id})";
        IsSeparateWindow = true;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        double? result = 0.0;
        if(_isPositive && _high.GetValue(bars[0]) - _high.GetValue(bars[1]) > _low.GetValue(bars[1]) - _low.GetValue(bars[0]))
        {
            result = _high.GetValue(bars[0]) - _high.GetValue(bars[1]);
        }
        else if (!_isPositive && _high.GetValue(bars[0]) - _high.GetValue(bars[1]) < _low.GetValue(bars[1]) - _low.GetValue(bars[0]))
        {
            result = _low.GetValue(bars[1]) - _low.GetValue(bars[0]);
        }
        return Math.Max(0, (double) result);
    }
}