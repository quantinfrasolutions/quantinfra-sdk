using NodaTime;

namespace QuantInfra.Sdk.MarketData;

public sealed class OrderBookSnapshot
{
    public OrderBookSnapshot(int contractId, Instant timestamp, IReadOnlyList<BookLevel> bids, IReadOnlyList<BookLevel> asks)
    {
        ContractId = contractId;
        Timestamp = timestamp;
        Bids = bids;
        Asks = asks;
    }

    public int ContractId { get; }
    public IReadOnlyList<BookLevel> Bids { get; }
    public IReadOnlyList<BookLevel> Asks { get; }
    public Instant Timestamp { get; }
}