using NodaTime;

namespace QuantInfra.Sdk.Strategies;

public interface IStrategyStateReadonly
{
    int StrategyId { get; }
    SignalGroup? ActiveSignalGroup { get; }
    Instant LastCalculationTs { get; }
    string InternalStateJson { get; }
    long Version { get; }
}