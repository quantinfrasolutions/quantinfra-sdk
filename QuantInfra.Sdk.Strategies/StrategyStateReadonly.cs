using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime;

namespace QuantInfra.Sdk.Strategies;

public class StrategyStateReadonly : IStrategyStateReadonly
{
    [JsonConstructor]
    public StrategyStateReadonly(
        int strategyId,
        SignalGroup? activeSignalGroup,
        Instant lastCalculationTs,
        string internalStateJson,
        long version
    )
    {
        StrategyId = strategyId;
        ActiveSignalGroup = activeSignalGroup;
        LastCalculationTs = lastCalculationTs;
        InternalStateJson = internalStateJson;
        Version = version;
    }

    public int StrategyId { get; }
    public SignalGroup? ActiveSignalGroup { get; protected set; }
    public Instant LastCalculationTs { get; }
    public string InternalStateJson { get; protected set; }
    public long Version { get; }
}

public static class StrategyStateExtensions
{
    public static TState? GetInternalState<TState>(this IStrategyStateReadonly state) => 
        JsonSerializer.Deserialize<TState>(state.InternalStateJson, JsonSerializerOptions);
    
    public static string SerializeInternalState(this object state) =>
        JsonSerializer.Serialize(state, JsonSerializerOptions);
    
    private static JsonSerializerOptions JsonSerializerOptions => new ()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        WriteIndented = false,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
    };
}