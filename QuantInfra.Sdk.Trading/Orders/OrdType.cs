namespace QuantInfra.Sdk.Trading.Orders;

public enum OrdType
{
    Market = 1,
    Limit,
    StopMarket,
    StopLimit,
    MarketIfTouched = 19
}

public static class OrdTypeExtensions
{
    public static bool IsPendingOrder(this OrdType o) =>
        o != OrdType.Market;
}