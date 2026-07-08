using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using QuantInfra.Sdk.StaticData;

namespace QuantInfra.Sdk.Backtesting;

public class TestUnit
{
    public TestUnit(
        string action,
        TestExecutorOptions options,
        PersistOptions persistOptions,
        string? metricsCalculatorName,
        string? data,
        string? metricsCalculorData,
        Instant createdAt,
        ContractOverride? contractOverride,
        Guid? testId = null
    )
    {
        TestId = testId ?? Guid.NewGuid();
        CreatedAt = createdAt;
        Action = action;
        Options = options;
        PersistOptions = persistOptions;
        MetricsCalculatorName = metricsCalculatorName;
        ContractOverride = contractOverride;
        Data = data ?? string.Empty;
        MetricsCalculorData = metricsCalculorData ?? string.Empty;
    }
    
    [JsonConstructor]
    public TestUnit() { }
    
    public TestUnit(
        Guid testId,
        string action,
        TestExecutorOptions options,
        PersistOptions persistOptions,
        ContractOverride? contractOverride,
        Instant createdAt,
        string? metricsCalculatorName,
        string data,
        string metricsCalculorData
    )
    {
        TestId = testId;
        CreatedAt = createdAt;
        MetricsCalculatorName = metricsCalculatorName;
        Action = action;
        Options = options;
        PersistOptions = persistOptions;
        ContractOverride = contractOverride;
        Data = data;
        MetricsCalculorData = metricsCalculorData;
    }

    public Guid TestId { get; init; }
    public Instant CreatedAt { get; init; }
    /// <summary>
    /// Name of the action to be executed in the test unit
    /// </summary>
    public string Action { get; init; }
    /// <summary>
    /// Options for the test
    /// </summary>
    public TestExecutorOptions Options { get; init; }
    /// <summary>
    /// Configuration of what results to collect and persist
    /// </summary>
    public PersistOptions PersistOptions { get; init; }
    /// <summary>
    /// Override properties for the contracts used in the test
    /// </summary>
    public ContractOverride? ContractOverride { get; init; }
    /// <summary>
    /// Name of the calculator that will be used to calculate metrics
    /// </summary>
    public string? MetricsCalculatorName { get; init; }
    public string Data { get; init; }
    public string MetricsCalculorData { get; init; }

    public TParams? GetParams<TParams>() => GetParams<TParams>(Data);
    public static TParams? GetParams<TParams>(string data) => JsonSerializer.Deserialize<TParams>(data, JsonSerializerOptions);
    public static string SerializeParams<TParams>(TParams @params) => JsonSerializer.Serialize(@params, JsonSerializerOptions);

    public TOptions? GetMetricsCalculatorOptions<TOptions>() => JsonSerializer.Deserialize<TOptions>(MetricsCalculorData, JsonSerializerOptions);
    
    private static readonly Lazy<JsonSerializerOptions> _jsonSerializerOptions = new(() =>
    {
        var options = new JsonSerializerOptions()
        {
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals | JsonNumberHandling.AllowReadingFromString,
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
            PropertyNameCaseInsensitive = true,
        };
        options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    });
    
    public static JsonSerializerOptions JsonSerializerOptions => _jsonSerializerOptions.Value;
}