using System;

namespace QuantInfra.Sdk.Backtesting;

public interface ITestResultsPersister
{
    void PersistResult(TestUnit unit, StrategyTestResult result);
    void PersistMetrics(TestUnit unit, ITestMetrics metrics);
}