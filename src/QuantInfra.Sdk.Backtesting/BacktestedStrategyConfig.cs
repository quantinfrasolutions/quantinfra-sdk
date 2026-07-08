using System.Collections.Generic;
using System.Text.Json.Serialization;
using QuantInfra.Sdk.Accounts;
using QuantInfra.Sdk.MarketData;
using QuantInfra.Sdk.Strategies;

namespace QuantInfra.Sdk.Backtesting;

public class BacktestedStrategyConfig : StrategyConfig
{
    [JsonConstructor]
    public BacktestedStrategyConfig() 
    {}
    
    public BacktestedStrategyConfig(
        string name,
        string className,
        string? @params,
        IReadOnlyDictionary<string, BarStorageConfig>? barStorages,
        IReadOnlyDictionary<string, int>? symbols,
        LiquidationParameters? liquidationParameters,
        bool useSignalGroups,
        int accountCurrencyId,
        PositionAccounting positionAccounting,
        bool includeUnrealizedPnLToMtm
    ) : base(name, className, @params, barStorages, symbols, liquidationParameters, useSignalGroups)
    {
        AccountCurrencyId = accountCurrencyId;
        PositionAccounting = positionAccounting;
        IncludeUnrealizedPnLToMtm = includeUnrealizedPnLToMtm;
    }

    public int AccountCurrencyId { get; init; }
    public PositionAccounting PositionAccounting { get; init; }
    public bool IncludeUnrealizedPnLToMtm { get; init; }
}