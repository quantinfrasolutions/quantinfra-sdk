using System.Collections.Generic;

namespace QuantInfra.Sdk.Backtesting;

public interface ITestExecutorFactory
{
    /// <summary>
    /// Returns a backtester for the given set of strategies 
    /// </summary>
    /// <param name="options">If not passed, the original options are used. The override is used mainly when you need to redefine the Start and End of the testing stage.</param>
    IBacktestRunner CreateExecutorInstance(IReadOnlyCollection<BacktestedStrategyConfig> configs, IActionProgressTracker? tracker = null, TestExecutorOptions? options = null);
}