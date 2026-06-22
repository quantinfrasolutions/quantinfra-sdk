using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Trading.ExternalAccounts;

public class OrderReplaceRequestExternal : OrderReplaceRequest, IExternalOrder
{
    public OrderReplaceRequestExternal() { }

    public OrderReplaceRequestExternal(OrderReplaceRequest req, string externalContractId, string externalOrderId, OrdType ordType) :
        base(req)
    {
        ExternalContractId = externalContractId;
        ExternalOrderId = externalOrderId;
        OrdType = ordType;
        Side = req.Side;
    }
    
    public string ExternalContractId { get; init; }
    public string ExternalOrderId { get; init; }
    public Side? Side { get; set; }
    public OrdType OrdType { get; set; }
}