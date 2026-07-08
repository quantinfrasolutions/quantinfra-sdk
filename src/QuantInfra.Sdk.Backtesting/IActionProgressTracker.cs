namespace QuantInfra.Sdk.Backtesting;

public interface IActionProgressTracker
{
    void SetCurrentProgress(double pct);
    void SetTestExecutionTime(long ms);
}