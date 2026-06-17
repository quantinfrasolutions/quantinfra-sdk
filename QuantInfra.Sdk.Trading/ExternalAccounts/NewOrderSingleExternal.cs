using NodaTime;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Trading.ExternalAccounts;

public class NewOrderSingleExternal : Order, IExternalOrder
{
    public NewOrderSingleExternal() { }

    public NewOrderSingleExternal(Order order, int brokerAccountId, string externalContractId, string? clOrdId = null) : base(order)
    {
        ExternalContractId = externalContractId;
        AccountId = brokerAccountId;
        BrokerAccountId = null;
        if (!string.IsNullOrEmpty(clOrdId)) ClOrdId = clOrdId;
    }

    public string ExternalContractId { get; init; }
    

    public ExternalExecutionReport ConfirmSentForExecution(string? externalOrderId, Instant ts) =>
        new ExternalExecutionReport(this, externalOrderId, OrdStatus.PendingNew, ExecType.PendingNew, ExecTypeReason.ExternalOrderIdAssignedToOrder, 
            null, null, 0, OrderQty, ts, null);

    public ExternalExecutionReport OutrightReject(Instant ts, ExecTypeReason? execTypeReason = null,
        RejectReason? rejectReason = null, string? rejectText = null) =>
        new ExternalExecutionReport(this, string.Empty, OrdStatus.Rejected, ExecType.Rejected, execTypeReason,
                null, null, 0, 0, ts, null)
            { RejectReason = rejectReason, RejectText = rejectText };
    
    protected bool Equals(NewOrderSingleExternal other) =>
        base.Equals(other) && ExternalContractId == other.ExternalContractId;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((NewOrderSingleExternal)obj);
    }
    
    public override string ToString() => 
        $"{{ NewOrderSingleExternal | {GetFormattingString()}, ExternalContractId: {ExternalContractId} }}";
}