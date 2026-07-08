using NodaTime;

namespace QuantInfra.Sdk.MarketData;

public interface IBar
{
    void AppendIndicatorData(int idHash, object data);
    object? GetIndicatorData(int idHash);
    void AppendIndicator(int idHash, double? value);
    double? this[int idHash] { get; }
    
    Instant OpenDt { get; }
    Instant CloseDt { get; }
    
    double? Open { get; }
    double? High { get; }
    double? Low { get; }
    double? Close { get; }
    double? Volume { get; }
    
    string BarToLogFormat { get; }
    ExchangeBar ToExchangeBar(int streamId, int? contractId);
}