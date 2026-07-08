using System.Text.Json.Serialization;
using NodaTime;

namespace QuantInfra.Sdk.Trading.Orders;

public class Order : NewOrderSingle
{
    public string AccountServiceName { get; init; }
    public long OrderId { get; init; }
    public string? ExternalId { get; init; }
    public int? BrokerAccountId { get; init; }
    /// <summary>
    /// All virtual orders are tracked inside VirtualExecutor and do not flow outside the platform
    /// </summary>
    public bool IsVirtual { get; init; }
    public new Instant CreatedAt { get; init; }

    [JsonConstructor] internal Order() { }

    public Order(NewOrderSingle order, string accountServiceName, long orderId) : base(order)
    {
        AccountServiceName = accountServiceName;
        OrderId = orderId;
    }
    
    public Order(Order o) : base(o)
    {
        AccountServiceName = o.AccountServiceName;
        OrderId = o.OrderId;
        ExternalId = o.ExternalId;
        BrokerAccountId = o.BrokerAccountId;
        IsVirtual = o.IsVirtual;
        CreatedAt = o.CreatedAt;
    }

    public static ExecutionReport CreateOrder(NewOrderSingle order, string accountServiceName, long orderId, long execId, Instant ts, bool isVirtual = false)
    {
        // TODO: add validation in NewOrderSingle
        if (order.OrderQty <= 0)
            return ExecutionReport.OutrightReject(order, accountServiceName, execId, RejectReason.IncorrectQuantity, "Order quantity must be positive", ts);
        
        return new ExecutionReport(
            new OrderStatus(order, accountServiceName, orderId, order.IsSuspended ? OrdStatus.Suspended : OrdStatus.PendingNew, 
                0, order.OrderQty, null, null
            ), 
            ExecType.PendingNew, execId, ts
        )
        {
            ExecTypeReason = ExecTypeReason.OrderChangeInitiated,
            LeavesQty = order.OrderQty,
            IsVirtual = isVirtual,
            CreatedAt = order.CreatedAt ?? ts
        };
    }

    public ExecutionReport AcceptOrder(long execId, Instant ts) => new(
        new OrderStatus(this, OrdStatus.New, 0, OrderQty, ts, null, null),
        ExecType.New, execId, ts
    );

    public ExecutionReport RejectOrder(long execId, RejectReason rejectReason, string rejectText, Instant ts) =>
        new(
            new OrderStatus(this, OrdStatus.Rejected, 0, 0, ts, rejectReason, rejectText),
            ExecType.Rejected, execId, ts
        );

    protected override string GetFormattingString() =>
        $"OrderId: {OrderId}, ExternalId: {ExternalId}, {base.GetFormattingString()}, BrokerAccountId: {BrokerAccountId}, IsVirtual: {IsVirtual}";

    public override string ToString() =>
        $"{{ Order | {GetFormattingString()} }}";

    protected override bool Equals(Order other) =>
        base.Equals(other) && OrderId == other.OrderId && ExternalId == other.ExternalId && BrokerAccountId == other.BrokerAccountId && IsVirtual == other.IsVirtual;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        // if (obj.GetType() != this.GetType()) return false;
        if (obj is not Order o) return false;
        return Equals(o);
    }
}