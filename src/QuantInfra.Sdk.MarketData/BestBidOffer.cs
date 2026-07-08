using NodaTime;

namespace QuantInfra.Sdk.MarketData;

public class BestBidOffer
{
    public BestBidOffer(BookLevel? bestBid, BookLevel? bestAsk, Instant lastUpdate)
    {
        BestBid = bestBid;
        BestAsk = bestAsk;
        LastUpdate = lastUpdate;
    }

    BookLevel? BestBid { get; }
    BookLevel? BestAsk { get; }
    Instant LastUpdate { get; }
}