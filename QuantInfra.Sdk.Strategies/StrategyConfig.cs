using System.Collections.Generic;
using System.Linq;
using QuantInfra.Sdk.MarketData;
using QuantInfra.Sdk.Utils;

namespace QuantInfra.Sdk.Strategies;

public class StrategyConfig : AlgorithmParams
{
    public int StrategyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ClassName { get; init; } = default!;
    public IReadOnlyDictionary<string, BarStorageConfig> RequiredBarStorages { get; init; } = default!;
    public IReadOnlyDictionary<string, int> Symbols { get; init; } = default!;
    public LiquidationParameters? LiquidationParameters { get; set; }
    public bool UseSignalGroups { get; init; }

    public StrategyConfig() { }
    
    public StrategyConfig(int strategyId, string name, string className, string? @params,
        IReadOnlyDictionary<string, BarStorageConfig>? barStorages, IReadOnlyDictionary<string, int>? symbols,
        LiquidationParameters? liquidationParameters, bool useSignalGroups
    ) : base(@params)
    {
        StrategyId = strategyId;
        Name = name;
        ClassName = className;
        RequiredBarStorages = barStorages ?? new Dictionary<string, BarStorageConfig>();
        Symbols = symbols ?? new Dictionary<string, int>();
        LiquidationParameters = liquidationParameters;
        UseSignalGroups = useSignalGroups;
    }

    public StrategyConfig(StrategyConfig s) : this(s.StrategyId, s.Name, s.ClassName, s.Params,
        s.RequiredBarStorages, s.Symbols, s.LiquidationParameters, s.UseSignalGroups)
    {
    }

    public IReadOnlyCollection<int> GetUsedContractsIds() => Symbols.Values
        .Union(RequiredBarStorages.Values.Where(bs => bs.IdType == IdType.Contract).Select(bs => bs.Id))
        .Distinct()
        .ToHashSet();

    // public bool Equals(StrategyConfig? other) =>
    //     other != null
    //     && other.StrategyId == StrategyId
    //     && other.StrategyClassName == StrategyClassName
    //
    //     && other.Params != null
    //     && other.Params.Count == Params.Count
    //     && !other.Params.Except(Params).Any()
    //
    //     && other.RequiredBarStorages != null
    //     && other.RequiredBarStorages.Count == RequiredBarStorages.Count
    //     && !other.RequiredBarStorages.Except(RequiredBarStorages).Any()
    //
    //     && other.Symbols != null
    //     && other.Symbols.Count == Symbols.Count
    //     && !other.Symbols.Except(Symbols).Any()
    //     
    //     && ((LiquidationParameters == null && other.LiquidationParameters == null)
    //         || LiquidationParameters!.Value.Equals(other.LiquidationParameters!.Value)
    //     );

    public override string ToString() =>
        $"<StrategyConfig {{ StrategyId={StrategyId}, Name={Name}, StrategyClassName={ClassName}, " +
        $"Params={Params}, " +
        $"RequiredBarStorages={{{string.Join(',', RequiredBarStorages.Select(p => $"{p.Key} = {p.Value}"))}}}, " +
        $"Symbols={{{string.Join(',', Symbols.Select(p => $"{p.Key} = {p.Value}"))}}} }}," +
        $"UseSignalGroups={UseSignalGroups} >";

    public string ToBriefString() =>
        $"{ClassName}({Symbols.Values.ToFormattedString()}, {RequiredBarStorages.Values.Select(bs => bs.Timeframe).ToFormattedString()})";
}