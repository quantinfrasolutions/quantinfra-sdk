using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime.Serialization.SystemTextJson;

namespace QuantInfra.Sdk.Strategies;

public class AlgorithmParams
{
    public AlgorithmParams() { }
    
    public AlgorithmParams(string? @params)
    {
        Params = @params ?? string.Empty;
    }

    public string Params { get; init; } = string.Empty;
    
    public TParams? DeserializeParams<TParams>() where TParams : class
        => DeserializeParams<TParams>(Params);

    public static TParams? DeserializeParams<TParams>(string @params) where TParams : class
    {
        try
        {
            return JsonSerializer.Deserialize<TParams>(@params, JsonSerializerOptions);
        }
        catch
        {
            return null;
        }
    }
    public static string SerializeParams(object? @params) => JsonSerializer.Serialize(@params, JsonSerializerOptions);
        
    // ReSharper disable once StaticMemberInGenericType
    private static JsonSerializerOptions? _jsonSerializerOptions;
    public static JsonSerializerOptions? JsonSerializerOptions
    {
        get
        {
            if (_jsonSerializerOptions == null)
            {
                _jsonSerializerOptions = new()
                {
                    NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals | JsonNumberHandling.AllowReadingFromString,
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
                    PropertyNameCaseInsensitive = true,
                };

                _jsonSerializerOptions.ConfigureForNodaTime(NodaTime.DateTimeZoneProviders.Tzdb);
                _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }

            return _jsonSerializerOptions;
        }
    }
}