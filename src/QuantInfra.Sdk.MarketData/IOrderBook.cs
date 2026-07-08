using NodaTime;

namespace QuantInfra.Sdk.MarketData;

public interface IOrderBookSide
{
    int Count { get; }
    IReadOnlyList<BookLevel> Levels { get; }
    bool TryGetBest(out BookLevel level);
    BookLevel? GetBest();
    bool TryGetLevelByPrice(decimal priceTicks, out BookLevel level, out int levelNumber);
    bool TryGetLevelByNumber(int levelNumber, out BookLevel level);
}

public interface IOrderBook
{
    IOrderBookSide Bids { get; }
    IOrderBookSide Asks { get; }
    Instant LastUpdate { get; }
}