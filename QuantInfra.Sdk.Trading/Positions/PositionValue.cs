using NodaTime;

namespace Common.Trading.Positions;

public record PositionValue(
    int AccountId,
    long PositionId,
    Instant Dt,
    decimal? Price,
    decimal SignedValue,
    decimal FxRate,
    decimal SignedValueInAccountCcy,
    decimal EquityValueInAccountCcy
);