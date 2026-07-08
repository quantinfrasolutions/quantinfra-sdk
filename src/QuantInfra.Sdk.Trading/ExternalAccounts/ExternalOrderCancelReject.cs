using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Trading.ExternalAccounts;

public class ExternalOrderCancelReject : OrderCancelReject
{
    public ExternalOrderCancelReject(int accountId, long? orderId, string? externalOrderId, string requestId, CxlRejReason rejectReason, string? rejectText)
        : base(accountId, orderId, requestId, rejectReason, rejectText)
    {
        ExternalOrderId = externalOrderId;
    }
    
    public string? ExternalOrderId { get; init; }
}