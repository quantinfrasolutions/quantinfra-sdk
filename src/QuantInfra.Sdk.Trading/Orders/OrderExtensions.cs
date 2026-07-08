using System;
using System.Collections.Generic;
using NodaTime;

namespace QuantInfra.Sdk.Trading.Orders;

/// <summary>
/// var sltp = Order.OneCancelsAnother(Order.LimitOrder(...), Order.StopOrder(...));
/// var order = Order
///     .LimitOrder(...)
///     .OnExecuted(sltp)
///     .OnRejected(ActionType.Replace, replayAttempts: 3) — TODO
/// </summary>
public static class OrderExtensions
{
    public static NewOrderSingle WithSuspendedActivation(this NewOrderSingle o, Instant activationDt) => new(o)
    {
        ActivationDt = activationDt,
        IsSuspended = true
    };
    
    public static Order[] OnExecuted(this Order o, Order linkedOrder, LinkType linkType)
    {
        var lo = o.GetExistingLinkedOrders();
        lo.Add(linkedOrder.ClOrdId, linkType);
        return new [] { new Order(o) { LinkedOrders = lo }, linkedOrder };
    }

    public static Order[] OnExecuted(this Order o, LinkType linkType, params Order[] orders)
    {
        var los = o.GetExistingLinkedOrders();
        foreach (var lo in orders)
        {
            los.Add(lo.ClOrdId, linkType);
        }

        var res = new Order[orders.Length + 1];
        res[0] = new Order(o) { LinkedOrders = los };
        orders.CopyTo(res, 1);
        return res;
    }

    public static Order[] OnCanceled(this Order o, LinkType linkType, params Order[] orders)
    {
        throw new NotImplementedException();
    }

    private static Dictionary<string, LinkType> GetExistingLinkedOrders(this Order o) =>
        o.LinkedOrders != null ? new Dictionary<string, LinkType>(o.LinkedOrders) : new();
    
    private static OrderReplaceRequest Replace(this Order o, decimal? orderQty = null, decimal? price = null, decimal? stopPx = null, string? requestId = null) => 
        new()
        {
            AccountId = o.AccountId,
            OrderId = o.OrderId != 0 ? o.OrderId : null,
            OrigClOrdId = o.OrderId != 0 ? null : o.ClOrdId,
            OrderQty = orderQty,
            Price = price,
            StopPx = stopPx,
            RequestId = requestId ?? Guid.NewGuid().ToString(),
        };
}