using NodaTime;

namespace QuantInfra.Sdk.Strategies;

public interface IStrategy
{
    void Stop(string reason);
    void UpdateInternalState(object? state);
    void UpdateLastCalculationTs(Instant ts);
}