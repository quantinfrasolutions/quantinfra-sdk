namespace QuantInfra.Sdk.Trading.Orders;

public enum ExecTypeReason
{
    NotSpecified = 0x0,
    /// <summary>
    /// When an order requested to be cancelled is not found on the execution venue
    /// </summary>
    UnsolicitedOrderCancellation = 0x4,
    /// <summary>
    /// When a new order is added to the account, requested to be canceled or replaced
    /// </summary>
    OrderChangeInitiated = 0xA,
    /// <summary>
    /// When an OrderCancelReject has been received for a PendingCancel order
    /// </summary>
    PendingCxlExecuted = 0xB,
    SuspendedOrderActivated = 0xD,
    BrokerAccountAssignedToOrder = 0x10,
    /// <summary>
    /// When an external order ID is assigned to the order by the trading client
    /// </summary>
    ExternalOrderIdAssignedToOrder = 0x11,
}