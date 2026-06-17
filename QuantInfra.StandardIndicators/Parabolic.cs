using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

public record struct ParabolicValue(
    bool BullishTrend,
    bool Reverse,
    double AccelerationUp,
    double AccelerationDown,
    double Highest,
    double Lowest,
    double? Value
)
{
    public override string ToString() =>
        $"{{\"BullishTrend\":{BullishTrend},\"Reverse\":{Reverse},\"AccelerationUp\":{AccelerationUp},\"AccelerationDown\":{AccelerationDown},\"Highest\":{Highest},\"Lowest\":{Lowest},\"Value\":{Value}}}";
}

/// <summary>
/// <para> Description </para> 
/// The indicator uses a trailing stop and reverse method called "SAR,"
/// or stop and reverse, to identify suitable exit and entry points.
/// Traders also refer to the indicator as to the parabolic stop and reverse,
/// parabolic SAR, or PSAR.
/// </summary>
public class Parabolic : AbstractIndicator<ParabolicValue>
{
    double _accelerationUp, _accelerationDown, _maxAcceleration;
    AbstractIndicator _high, _low;

    public Parabolic(
        AbstractIndicator high = null,
        AbstractIndicator low = null,
        double accelerationUp = 0.25,
        double accelerationDown = 0.25
    )
    {
        _high = high ?? new High();
        _low = low ?? new Low();
        _accelerationUp = accelerationUp;
        _accelerationDown = accelerationDown;
        _maxAcceleration = Math.Max(accelerationUp, accelerationDown) * 6;
        RegisterIndicator(_high, 1);
        RegisterIndicator(_low, 1);

        Id = $"Parabolic:{_accelerationUp}:{_accelerationDown}({_high.Id},{_low.Id})";
        WarmupBars = 1;
    }


    protected override (double?, ParabolicValue?) CalculateAndPersistData(IBarStorage bars, double? price = null)
    {
        if (bars.Count < 2 || GetValue(bars[1]) == null)
        {
            var value = new ParabolicValue(
                true,
                false,
                _accelerationUp,
                _accelerationDown,
                _high.GetValue(bars[0]) ?? 0,
                _low.GetValue(bars[0]) ?? 0,
                _low.GetValue(bars[0]) ?? 0
            );

            return (value.Value, value);
        }

        var previousValue = GetData(bars[1]).Value!;

        var res = previousValue;

        if (previousValue.BullishTrend)
        {
            if (_low.GetValue(bars[0]) < previousValue.Value)
            {
                res = previousValue with
                {
                    BullishTrend = false,
                    Reverse = true,
                    AccelerationUp = _accelerationUp,
                    AccelerationDown = _accelerationDown,
                    Highest = _high.GetValue(bars[0]) ?? 0,
                    Lowest = _low.GetValue(bars[0]) ?? 0,
                    Value = previousValue.Highest
                };

                return (res.Value, res);
            }
            else
            {
                if ((_high.GetValue(bars[0]) ?? 0) > previousValue.Highest)
                {
                    res = res with
                    {
                        Highest = _high.GetValue(bars[0]) ?? 0
                    };
                        
                    if (!previousValue.Reverse)
                    {
                        res = res with
                        {
                            AccelerationUp = Math.Min(res.AccelerationUp + _accelerationUp, _maxAcceleration)
                        };
                    }
                }

                double? delta = res.Highest - previousValue.Value;
                delta *= res.AccelerationUp;

                res = res with
                {
                    Reverse = false,
                    Value = previousValue.Value + delta
                };


                double lowest = Math.Min(_low.GetValue(bars[0]) ?? 0, _low.GetValue(bars[1]) ?? 0);
                if (res.Value > lowest)
                {
                    res = res with
                    {
                        Value = lowest
                    };
                }
            }
        }
        else
        {
            if ((_high.GetValue(bars[0]) ?? 0) > previousValue.Value)
            {
                res = res with
                {
                    BullishTrend = true,
                    Reverse = true,
                    AccelerationUp = _accelerationUp,
                    AccelerationDown = _accelerationDown,
                    Highest = _high.GetValue(bars[0]) ?? 0,
                    Lowest = _low.GetValue(bars[0]) ?? 0,
                    Value = previousValue.Lowest
                };

                return (res.Value, res);
                    
            }
            else
            {
                if (_low.GetValue(bars[0]) < previousValue.Lowest)
                {
                    res = res with
                    {
                        Lowest = _low.GetValue(bars[0]) ?? 0
                    };

                    if (!previousValue.Reverse)
                    {
                        res = res with
                        {
                            AccelerationDown = Math.Min(res.AccelerationDown + _accelerationDown, _maxAcceleration)
                        };
                    }
                }
                    
                double? delta = res.Lowest - previousValue.Value;
                delta *= res.AccelerationDown;

                res = res with
                {
                    Reverse = false,
                    Value = previousValue.Value + delta
                };

                double highest = Math.Max(_high.GetValue(bars[0]) ?? 0, _high.GetValue(bars[1]) ?? 0);

                if (res.Value < highest)
                {
                    res = res with
                    {
                        Value = highest
                    };
                }
            }
        }

        return (res.Value, res);
    }
}