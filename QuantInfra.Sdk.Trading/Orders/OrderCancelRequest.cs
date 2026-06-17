namespace QuantInfra.Sdk.Trading.Orders;

public class OrderCancelRequest
{
    public OrderCancelRequest() { }

    public OrderCancelRequest(int accountId, string clOrdId, long? orderId, string? origClOrdId)
    {
        AccountId = accountId;
        ClOrdId = clOrdId;
        OrderId = orderId;
        OrigClOrdId = origClOrdId;
    }

    public OrderCancelRequest(OrderCancelRequest r) : this(r.AccountId, r.ClOrdId, r.OrderId, r.OrigClOrdId)
    { }
    
    public int AccountId { get; init; }
    public string ClOrdId { get; init; }
    public long? OrderId { get; init; }
    public string? OrigClOrdId { get; init; }
}