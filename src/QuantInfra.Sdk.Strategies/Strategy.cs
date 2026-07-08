using System.Collections.Generic;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.Sdk.Strategies;

public class Strategy : StrategyConfig
{
    public Strategy() { }
        
    public Strategy(int strategyId, string name, string className, string? @params,
        IReadOnlyDictionary<string, BarStorageConfig>? barStorages, IReadOnlyDictionary<string, int>? symbols,
        LiquidationParameters? liquidationParameters, bool useSignalGroups,
        StrategyStatus status, int accountId, string? strategyServiceName
    ) : base(name, className, @params, barStorages, symbols, liquidationParameters, useSignalGroups)
    {
        StrategyId = strategyId;
        Status = status;
        AccountId = accountId;
        StrategyServiceName = strategyServiceName;
    }
        
    public int StrategyId { get; init; }
    public StrategyStatus Status { get; set; }
    public int AccountId { get; init; }
    public string? StrategyServiceName { get; init; }
}