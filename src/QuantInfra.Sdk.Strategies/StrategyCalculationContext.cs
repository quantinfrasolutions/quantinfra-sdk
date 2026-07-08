using System.Collections.Generic;
using NodaTime;
using QuantInfra.Sdk.Accounts;
using QuantInfra.Sdk.Accounts.AccountStates;
using QuantInfra.Sdk.StaticData;

namespace QuantInfra.Sdk.Strategies;

public abstract class StrategyCalculationContext
{
    protected StrategyCalculationContext(AccountRecordV6 accountRecord, IAccountStateReadonly accountState, Strategy strategyRecord, IStrategyStateReadonly strategyState, Instant referenceDt, Instant processingDt, bool isHistory, int virtualAccountSizeStepFraction)
    {
        AccountRecord = accountRecord;
        AccountState = accountState;
        StrategyRecord = strategyRecord;
        StrategyState = strategyState;
        ReferenceDt = referenceDt;
        ProcessingDt = processingDt;
        IsHistory = isHistory;
        VirtualAccountSizeStepFraction = virtualAccountSizeStepFraction;
    }

    public AccountRecordV6 AccountRecord { get; }
    public IAccountStateReadonly AccountState { get; }
    
    public Strategy StrategyRecord { get; }
    public IStrategyStateReadonly StrategyState { get; }
    
    public Instant ReferenceDt { get; set; }
    public Instant ProcessingDt { get; }
    public bool IsHistory { get; }
    public int VirtualAccountSizeStepFraction { get; }
    public abstract Contract? GetContract(int contractId);
}

public abstract class HostedStrategyCalculationContext : StrategyCalculationContext
{
    public HostedStrategyCalculationContext(
        Strategy? strategyRecord, 
        IStrategyStateReadonly? strategyState,
        IAccountStateReadonly? accountState,
        IStrategy? strategy,
        ITradingAccount? account,
        AccountRecordV6? accountRecord,
        Instant referenceDt,
        Instant processingDt,
        bool isHistory,
        bool throwOnZeroVolumeOrders = true,
        int virtualAccountSizeStepFraction = 100
    ) : base(accountRecord, accountState, strategyRecord, strategyState, referenceDt, processingDt, isHistory, virtualAccountSizeStepFraction)
    {
        Strategy = strategy;
        Account = account;
        ReferenceDt = referenceDt;
        ThrowOnZeroVolumeOrders = throwOnZeroVolumeOrders;
    }

    
    
    public IStrategy Strategy { get; }
    public ITradingAccount Account { get; }
    public bool ThrowOnZeroVolumeOrders { get;}
    
    public bool IsOk => StrategyRecord is not null && StrategyState is not null && Strategy is not null
        && AccountRecord is not null && AccountState is not null && Account is not null;

    public string GetMissingInfoLogString()
    {
        var failedFields = new List<string>(6);
        if (StrategyRecord is null) failedFields.Add("StrategyRecord");
        if (StrategyState is null) failedFields.Add("StrategyState");
        if (Strategy is null) failedFields.Add("Strategy");
        if (AccountRecord is null) failedFields.Add("AccountRecord");
        if (AccountState is null) failedFields.Add("AccountState");
        if (Account is null) failedFields.Add("Account");
        
        return string.Join(", ", failedFields);
    }
}