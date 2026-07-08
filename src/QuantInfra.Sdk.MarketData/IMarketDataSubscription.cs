namespace QuantInfra.Sdk.MarketData;

public interface ISubscription
{
    int SubscriptionId { get; }
}

public interface IMarketDataSubscription : ISubscription
{
    int? StreamId { get; }
    SubscriptionType SubscriptionType { get; }
}

public interface IOrderBookSubscription : ISubscription
{
    int ContractId { get; }
}

public enum SubscriptionType
{
    Trades,
    Midpoint,
    Candles1M,
}