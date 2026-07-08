using System.Collections.Generic;

namespace QuantInfra.Sdk.Backtesting;

/// <summary>
/// Implement this interface to define custom testing actions (e.g., optimization)
/// </summary>
public interface IStrategyTestAction
{
    string Name { get; }
    void Run(TestUnit unit, ITestExecutorFactory teFactory, IActionProgressTracker tracker, IMetricsCalculator? metricsCalculator, ITestResultsPersister persister);

    /// <summary>
    /// This method is called by the tester when market data is validated before the run.
    /// </summary>
    /// <returns>The list of streams/contracts that will be used by the strategies in the test.</returns>
    IReadOnlyCollection<MarketDataRequirement> GetMarketDataRequirements(TestUnit unit);
    
    /// <summary>
    /// Validates test action parameters submitted by the user
    /// </summary>
    /// <param name="options"></param>
    /// <param name="availableStrategyTypes"></param>
    /// <returns></returns>
    ActionParamsValidationResult ValidateParams(string? options, IReadOnlyCollection<string> availableStrategyTypes);
    
    /// <summary>
    /// Returns an example of parameters to be shown in the TesterUI
    /// </summary>
    /// <returns></returns>
    string GetSampleParams();
}