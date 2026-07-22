using System;
using System.Collections.Generic;
using System.Linq;
using QuantInfra.Sdk.Trading;
using QuantInfra.Sdk.Trading.Orders;
using QuantInfra.Sdk.Trading.Positions;

namespace QuantInfra.Sdk.Trading;

public static class Extensions
{
    public static OrderStatus? GetOrder(this IEnumerable<OrderStatus> orders, long contractId, string clOrdId, bool includeSuspended = false) =>
        orders.SingleOrDefault(o =>
            o.ContractId == contractId
            && o.ClOrdId == clOrdId
            && (!includeSuspended || o.OrdStatus != OrdStatus.Suspended)
        );
    
    public static OrderStatus? GetOrder(this IEnumerable<OrderStatus> orders, long orderId) => orders.SingleOrDefault(o => o.OrderId == orderId);
    
    public static OrderStatus? GetOrder(this IEnumerable<OrderStatus> orders, long? orderId, string? clOrdId)
    {
        if (orderId.HasValue && clOrdId != null) 
            throw new InvalidOperationException("One of orderId or requestClOrdId must be provided");
        
        return orders.SingleOrDefault(o =>
            (orderId.HasValue && o.OrderId == orderId.Value)
            || (!string.IsNullOrEmpty(clOrdId) && o.ClOrdId == clOrdId)
        );
    }

    public static IEnumerable<OrderStatus> GetOrdersByContractId(this IEnumerable<OrderStatus> orders, long contractId) =>
        orders.Where(o => o.ContractId == contractId);
    
    public static Position? GetPosition(this IEnumerable<Position> positions, long contractId, string? strategyPositionId) =>
        positions.SingleOrDefault(p => p.ContractId == contractId && p.StrategyPositionId == strategyPositionId);
    
    public static IEnumerable<Position> GetPositionsByContractId(this IEnumerable<Position> positions, long contractId) =>
        positions.Where(p => p.ContractId == contractId);

    public static NewOrderSingle GetClosingOrder(this Position p, string? clOrdId = null) =>
        NewOrderSingle.MarketOrder(
            clOrdId,
            p.AccountId,
            p.ContractId,
            p.StrategyPositionId,
            PositionEffect.Close,
            p.Volume,
            p.Side.Invert()
        );
}