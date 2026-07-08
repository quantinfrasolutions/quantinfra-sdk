namespace QuantInfra.Sdk.Trading.Orders;

public class OrderCancelReject
{
    public OrderCancelReject(int accountId, long? orderId, string requestId, CxlRejReason rejectReason, string? rejectText = null)
    {
        AccountId = accountId;
        OrderId = orderId;
        RequestId = requestId;
        RejectReason = rejectReason;
        RejectText = rejectText;
    }

    public int AccountId { get; init; }
    public long? OrderId { get; init; }
    public string RequestId { get; init; }
    public CxlRejReason RejectReason { get; init; }
    public string? RejectText { get; init; }
}