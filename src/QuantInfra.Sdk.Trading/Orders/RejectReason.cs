namespace QuantInfra.Sdk.Trading.Orders;

public enum RejectReason
{
    NotSpecified = 0,
    UnknownSymbol = 1,
    ExchangeClosed = 2,
    OrderExceedsLimit = 3,
    ApplicationNotAvailable = 4,
    /// <summary>
    /// Used to remove orders during reconciliation
    /// </summary>
    UnknownOrder = 5,
    DuplicateOrder = 6,
    IncorrectQuantity = 13,
    UnknownAccount = 15,
    UnavailablePriceOrLiquidity = 32,
    DuplicateVirtualAccountExecution,
    Other = 99,
    TooLateToCancel = 102,
}