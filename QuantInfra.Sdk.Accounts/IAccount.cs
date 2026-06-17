using System;
using System.Collections.Generic;
using Common.Trading;
using NodaTime;
using QuantInfra.Sdk.Accounting;
using QuantInfra.Sdk.Accounts.AccountStates;
using QuantInfra.Sdk.Trading.Orders;

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
    
    void MarkToMarketEod(IReadOnlyDictionary<int, decimal> eodPrices, Instant dt, Instant processingDt);
}