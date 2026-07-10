using System;
using System.Collections.Generic;
using NodaTime;
using QuantInfra.Sdk.Accounting;
using QuantInfra.Sdk.Accounts.AccountStates;
using QuantInfra.Sdk.Trading;
using QuantInfra.Sdk.Trading.Orders;
using QuantInfra.Sdk.Trading.Positions;

namespace QuantInfra.Sdk.Accounts;

public interface IAccount : ITradingAccount
{
    IAccountStateReadonly AccountStateReadonly { get; }
    
    AccountType AccountType { get; }
    int AccountId { get; }
    
    decimal? GetBaseTradeSize(int contractId);
    decimal GetInvestment();
    
    void CreateAccount(Instant dt);
    
    void ProcessBalanceOperation(NewBalanceOperation request, Instant processingDt, Guid? requestId = null);
    void ProcessExecutionReport(ExecutionReport? er, Instant processingDt);
    void ProcessTrade(Trade trade, Instant processingDt);
    
    void OnHeartbeat(Instant dt);
    
    (decimal dailyReturn, decimal currentDrawdown) GetLiquidationInfo(IReadOnlyDictionary<int, double> lastPrices, Instant referenceDt);

    public (
        IReadOnlyCollection<Position> positions,
        IReadOnlyDictionary<long, PositionValue> positionValues,
        IReadOnlyDictionary<int, BalanceValue> balanceValues,
        IReadOnlyDictionary<int, PnLCalculatorOptions> pnLCalculatorOptions,
        bool success
    ) MarkToMarket(Instant dt);
    
    void MarkToMarketEod(IReadOnlyDictionary<int, decimal> eodPrices, Instant dt, Instant processingDt);
}