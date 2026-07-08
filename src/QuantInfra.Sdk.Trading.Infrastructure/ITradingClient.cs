using QuantInfra.Sdk.Trading.ExternalAccounts;

namespace QuantInfra.Sdk.Trading.Infrastructure;

public interface ITradingClient
{
    void PlaceOrder(NewOrderSingleExternal order);
    void CancelOrder(OrderCancelRequestExternal ocr);
    void ReplaceOrder(OrderReplaceRequestExternal ocr);
}