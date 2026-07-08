using System;
using System.Collections.Generic;

namespace QuantInfra.Sdk.Strategies;

public struct LiquidationParameters
{
    private sealed class LiquidationParametersEqualityComparer : IEqualityComparer<LiquidationParameters>
    {
        public bool Equals(LiquidationParameters x, LiquidationParameters y)
        {
            return x.Drawdown == y.Drawdown && x.DailyDrawdown == y.DailyDrawdown && x.LiquidateAt == y.LiquidateAt;
        }

        public int GetHashCode(LiquidationParameters obj)
        {
            return HashCode.Combine(obj.Drawdown, obj.DailyDrawdown, (int)obj.LiquidateAt);
        }
    }

    public static IEqualityComparer<LiquidationParameters> LiquidationParametersComparer { get; } = new LiquidationParametersEqualityComparer();

    public decimal? Drawdown { get; init; }
    public decimal? DailyDrawdown { get; init; }
    public LiquidationTiming LiquidateAt { get; init; }
}