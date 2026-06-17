using Common.Trading;
using NodaTime;

namespace QuantInfra.Sdk.MarketData;

public class ExchangeTrade
{
    public Instant Dt { get; init; }
    public int StreamId { get; init; }        
    public double Price { get; init; }
    public Side Side { get; init; }
    public double Volume { get; init; }
    public int? ContractId { get; init; }
    public int? TradingSessionId { get; init; }
}