using System;
using System.Collections.Generic;
using QuantInfra.Sdk.Accounting;
using QuantInfra.Sdk.Trading;
using QuantInfra.Sdk.Trading.Positions;

namespace QuantInfra.Sdk.Backtesting;

public interface ITestMetrics
{
    Guid TestId { get; }
    int? StrategyId { get; }
    string? Label { get; }
}

public interface IMetricsCalculator
{
    string Name { get; }

    string GetSampleOptions();
    
    void CalculateAndPersist(
        ITestResultsPersister perister, 
        TestUnit testUnit, int? strategyId, string? label,
        IEnumerable<Trade> trades, IEnumerable<Position> positionCloses, IEnumerable<SharePriceHistory> returns
    );
}

public interface IMetricsCalculator<in TOptions, out TResult> : IMetricsCalculator
    where TResult : ITestMetrics
{
    TResult Get(TOptions options, Guid testId, int? strategyId, string? label, 
        IEnumerable<Trade> trades, IEnumerable<Position> positionCloses, IEnumerable<SharePriceHistory> returns
    );
}