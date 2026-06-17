using Common.Trading;

namespace QuantInfra.Sdk.Trading.Orders;

public class OrderReplaceRequest
{
    public OrderReplaceRequest() { }

    public OrderReplaceRequest(OrderReplaceRequest r)
    {
        AccountId = r.AccountId;
        RequestId = r.RequestId;
        OrderId = r.OrderId;
        OrigClOrdId = r.OrigClOrdId;
        Price = r.Price;
        StopPx = r.StopPx;
        OrderQty = r.OrderQty;
        Side = r.Side;
    }
    
    public int AccountId { get; init; }
    public string RequestId { get; init; }
    public long? OrderId { get; init; }
    public string? OrigClOrdId { get; init; }
    public decimal? Price { get; init; }
    public decimal? StopPx { get; init; }
    public decimal? OrderQty { get; init; }
    public Side? Side { get; init; }
}