namespace QuantInfra.Sdk.Strategies;

public enum StrategyStatus
{
    Stopped,
    Running,
    Paused,
    Stopping
}

public static class StrategyStatusExtensions
{
    public static bool IsActive(this StrategyStatus status) => status == StrategyStatus.Running || status == StrategyStatus.Stopping;
}