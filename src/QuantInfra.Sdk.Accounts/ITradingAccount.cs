using NodaTime;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Accounts;

public interface ITradingAccount
{
    void PlaceOrder(NewOrderSingle order, Instant processingDt);
    void CancelOrder(OrderCancelRequest request, Instant processingDt);
    void ReplaceOrder(OrderReplaceRequest request, Instant processingDt);
}