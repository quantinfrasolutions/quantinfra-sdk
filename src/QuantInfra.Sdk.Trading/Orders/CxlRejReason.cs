namespace QuantInfra.Sdk.Trading.Orders;

public enum CxlRejReason
{
    /// <summary>
    /// When an order requested to be canceled or replaced does not exist — means a successful cancellation
    /// </summary>
    UnknownOrder = 1,
    /// <summary>
    /// When the replace request doesn't change the order — means a successful replace
    /// </summary>
    OrderUnchanged = 2,
    Other = 99,
}