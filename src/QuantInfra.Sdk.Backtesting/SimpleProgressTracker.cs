namespace QuantInfra.Sdk.Backtesting;

public class SimpleProgressTracker : IActionProgressTracker
{
    public double CurrentProgress { get; private set; }
    public long ExecutionTime { get; private set; }

    public void SetCurrentProgress(double pct) => CurrentProgress = pct;
    public void SetTestExecutionTime(long ms) => ExecutionTime = ms;
}