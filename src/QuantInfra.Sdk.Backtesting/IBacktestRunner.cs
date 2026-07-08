namespace QuantInfra.Sdk.Backtesting;

public interface IBacktestRunner
{
    /// <summary>
    /// Run the test
    /// </summary>
    void Run();

    StrategyTestResult GetResult();
}