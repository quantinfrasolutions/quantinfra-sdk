using System;
using NodaTime;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Trading.ExternalAccounts;

public class OrderCancelRequestExternal : IExternalOrder
{
    public OrderCancelRequestExternal() { }

    public OrderCancelRequestExternal(long? orderId, int accountId, string externalContractId, string? externalOrderId)
    {
        if (!orderId.HasValue && string.IsNullOrEmpty(externalOrderId))
        {
            throw new InvalidOperationException("Either OrderId or ExternalOrderId must be provided");
        }
        OrderId = orderId;
        AccountId = accountId;
        ExternalContractId = externalContractId;
        ExternalOrderId = externalOrderId;
    }

    public long? OrderId { get; init; }
    public int AccountId { get; init; }
    public ExecutionReport RejectOrder(RejectReason rejectReason, string rejectReasonStr, Instant processingDt)
    {
        throw new NotImplementedException();
    }

    public string ExternalContractId { get; init; }
    public string? ExternalOrderId { get; init; }
    
    
    protected bool Equals(OrderCancelRequestExternal other)
    {
        return Nullable.Equals(OrderId, other.OrderId) && AccountId.Equals(other.AccountId) && ExternalContractId == other.ExternalContractId && ExternalOrderId == other.ExternalOrderId;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((OrderCancelRequestExternal)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(OrderId, AccountId, ExternalContractId, ExternalOrderId);
    }

    public override string ToString()
    {
        return $"{{ OrderCancelRequestExternal | OrderId: {OrderId}, AccountId: {AccountId}, ExternalContractId: {ExternalContractId}, ExternalOrderId: {ExternalOrderId} }}";
    }
}