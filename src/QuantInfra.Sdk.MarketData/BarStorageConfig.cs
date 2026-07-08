using System.Text;
using System.Text.Json.Serialization;
using NodaTime;

namespace QuantInfra.Sdk.MarketData;

public class BarStorageConfig
{
    public IdType IdType { get; set; } = IdType.Contract;
    public int Id { get; set; }
    public BarAggregationType AggregationType { get; set; }
    public int[]? TradingSessionIds { get; set; }
    /// <summary>
    /// Use Roundtrip pattern: https://nodatime.org/3.1.x/userguide/period-patterns
    /// </summary>
    public Period Timeframe { get; set; } = Period.FromMinutes(1);
    /// <summary>
    /// Use Roundtrip pattern: https://nodatime.org/3.1.x/userguide/period-patterns
    /// </summary>
    public Period Offset { get; set; } = Period.Zero;
    /// <summary>
    /// Use Tzdb provider: https://nodatime.org/TimeZones
    /// </summary>
    public string Timezone { get; set; } = "UTC";
    public bool LastValueOnly { get; set; }

    string _fullQualifier;
    [JsonIgnore]
    public string FullQualifier
    {
        get
        {
            if (string.IsNullOrEmpty(_fullQualifier))
                _fullQualifier = new StringBuilder()
                    .Append($"{IdType}.{Id}.{AggregationType}.{Timeframe}.{Timezone}.{Offset}.{LastValueOnly}")
                    .Append(TradingSessionIds != null ? $".{string.Join(",", TradingSessionIds)}" : string.Empty)
                    .ToString();

            return _fullQualifier;
        }
    }

    public BarStorageConfig() { }
    public BarStorageConfig(BarStorageConfig bs)
    {
        IdType = bs.IdType;
        Id = bs.Id;
        AggregationType = bs.AggregationType;
        TradingSessionIds = bs.TradingSessionIds?.ToArray();
        Timeframe = bs.Timeframe;
        Offset = bs.Offset;
        Timezone = bs.Timezone;
        LastValueOnly = bs.LastValueOnly;
    }
}

public enum IdType
{
    Stream = 1,
    Contract = 2
}