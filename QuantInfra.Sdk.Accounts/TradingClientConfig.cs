using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime.Serialization.SystemTextJson;

namespace QuantInfra.Sdk.Accounts;

public class TradingClientConfig
{
    public int AccountId { get; init; }
    public string ExecutionServiceName { get; init; }
    public string? ExternalAccountId { get; init; }
    public string TradingClientClassName { get; init; }
    public string? TradingClientParamsSerialized { get; init; }
    public string TradingClientSecret { get; init; }
    public bool WritePerformanceMetrics { get; set; }

    public TParams? GetParams<TParams>() =>
        string.IsNullOrEmpty(TradingClientParamsSerialized) 
            ? default 
            : JsonSerializer.Deserialize<TParams>(TradingClientParamsSerialized, JsonSerializerOptions);

    public object? GetParams(Type paramsType) =>
        string.IsNullOrEmpty(TradingClientParamsSerialized) 
            ? null 
            : JsonSerializer.Deserialize(TradingClientParamsSerialized, paramsType, JsonSerializerOptions);

    private static Lazy<JsonSerializerOptions> _jsonSerializerOptions = new(() =>
    {
        var jsonSerializerOptions = new JsonSerializerOptions()
        {
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals |
                             JsonNumberHandling.AllowReadingFromString,
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
            PropertyNameCaseInsensitive = true,
        };

        jsonSerializerOptions.ConfigureForNodaTime(NodaTime.DateTimeZoneProviders.Tzdb);
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        return jsonSerializerOptions;
    });
        
    public static JsonSerializerOptions? JsonSerializerOptions => _jsonSerializerOptions.Value;

    protected bool Equals(TradingClientConfig other)
    {
        return AccountId == other.AccountId && ExecutionServiceName == other.ExecutionServiceName && ExternalAccountId == other.ExternalAccountId && TradingClientClassName == other.TradingClientClassName && TradingClientParamsSerialized == other.TradingClientParamsSerialized && TradingClientSecret == other.TradingClientSecret;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((TradingClientConfig)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(AccountId, ExecutionServiceName, ExternalAccountId, TradingClientClassName, TradingClientParamsSerialized, TradingClientSecret);
    }
}