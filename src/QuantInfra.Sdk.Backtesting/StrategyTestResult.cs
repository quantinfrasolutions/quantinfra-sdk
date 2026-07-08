using System.Collections.Generic;
using QuantInfra.Sdk.Accounting;
using QuantInfra.Sdk.Accounts;
using QuantInfra.Sdk.Strategies;
using QuantInfra.Sdk.Trading;
using QuantInfra.Sdk.Trading.Positions;

namespace QuantInfra.Sdk.Backtesting;

public record StrategyTestResult(
    IReadOnlyCollection<Strategy> StrategyConfigs, 
    IReadOnlyCollection<AccountRecordV6> Accounts,
    IReadOnlyList<SharePriceHistory> Returns, 
    IReadOnlyList<Trade> Trades, 
    IReadOnlyList<Position> PositionCloses,
    IReadOnlyList<Position> EndOfDayPositions,
    IReadOnlyList<BalanceValue> EndOfDayBalancesAggregated,
    IReadOnlyList<BalanceValue> EndOfDayBalances,
    IReadOnlyList<PositionValue> PositionValues,
    IReadOnlyList<Commission> Commissions
);