namespace QuantInfra.Sdk.Trading.Orders;

public enum TimeInForce
{
    GoodTillCancelled,
    ImmediateOrCancel,
    FillOrKill,
    GoodTillTime,
    Unknown
}