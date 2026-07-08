namespace QuantInfra.Sdk.MarketData;

public interface IOrderBooksProvider
{
    /// <summary>
    /// Get current order book state by contract id. Returns null if order book is not available.
    /// </summary>
    IOrderBook? this[int contractId] { get; }
    void ClaimOrderBook(int contractId, int? strategyId = null);
}

public interface IBestBidOfferProvider
{
    /// <summary>
    /// Get latest BBO by contract id. Returns null if BBO is not available.
    /// </summary>
    /// <param name="contractId"></param>
    BestBidOffer? this[int contractId] { get; }
    void ClaimBestBidOffer(int contractId, int? strategyId = null);
}