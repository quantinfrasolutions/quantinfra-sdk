using System.Collections.Generic;
using Common.Trading;
using NodaTime;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Trading.ExternalAccounts;

public class ExternalExecutionReport
{
    public ExternalExecutionReport() { }
    
    public ExternalExecutionReport(
        string? clOrdId,
        long? orderId,
        string? externalId,
        int accountId,
        string? externalContractId,
        OrdStatus ordStatus,
        OrdType? ordType,
        Side? side,
        decimal? lastQty,
        decimal? lastPx,
        decimal? orderQty,
        decimal cumQty,
        decimal leavesQty,
        decimal? price,
        decimal? stopPx,
        TimeInForce? timeInForce,
        Instant? expireDt,
        ExecType execType,
        ExecTypeReason? execTypeReason,
        RejectReason? rejectReason,
        string? rejectText,
        Instant transactTime,
        decimal? calculatedCcyLastQty
    )
    {
        ClOrdId = clOrdId;
        OrderId = orderId;
        ExternalId = externalId;
        AccountId = accountId;
        ExternalContractId = externalContractId;
        OrdStatus = ordStatus;
        OrdType = ordType;
        Side = side;
        LastQty = lastQty;
        LastPx = lastPx;
        OrderQty = orderQty;
        CumQty = cumQty;
        LeavesQty = leavesQty;
        Price = price;
        StopPx = stopPx;
        TimeInForce = timeInForce;
        ExpireDt = expireDt;
        ExecType = execType;
        ExecTypeReason = execTypeReason;
        RejectReason = rejectReason;
        RejectText = rejectText;
        TransactTime = transactTime;
        CalculatedCcyLastQty = calculatedCcyLastQty;
    }
    
    public ExternalExecutionReport(NewOrderSingleExternal order, string externalId, OrdStatus ordStatus, 
        ExecType execType, ExecTypeReason? execTypeReason, decimal? lastQty, decimal? lastPx, decimal cumQty, decimal leavesQty, Instant transactTime, decimal? calculatedCcyLastQty
    ) : this(order.ClOrdId, order.OrderId, externalId, order.AccountId, order.ExternalContractId, 
        ordStatus, order.OrdType, order.Side, lastQty, lastPx, order.OrderQty, cumQty, leavesQty, order.Price, order.StopPx,
        order.TimeInForce, order.ExpireDt, execType, execTypeReason, null, null, transactTime, calculatedCcyLastQty)
    {
        
    }

    public ExternalExecutionReport(ExternalExecutionReport er) : this(er.ClOrdId, er.OrderId, er.ExternalId, er.AccountId, er.ExternalContractId, er.OrdStatus, er.OrdType,
        er.Side, er.LastQty, er.LastPx, er.OrderQty, er.CumQty, er.LeavesQty, er.Price, er.StopPx,
        er.TimeInForce, er.ExpireDt, er.ExecType, er.ExecTypeReason, er.RejectReason, er.RejectText,
        er.TransactTime, er.CalculatedCcyLastQty) 
    { }

    public ExternalExecutionReport(ExecutionReport er, string externalContractId) : this(er.ClOrdId, er.OrderId, er.ExternalId,
        er.AccountId, externalContractId, er.OrdStatus, er.OrdType, er.Side, er.LastQty, er.LastPx, er.OrderQty, er.CumQty,
        er.LeavesQty, er.Price, er.StopPx, er.TimeInForce, er.ExpireDt, er.ExecType, er.ExecTypeReason, er.RejectReason, er.RejectText,
        er.TransactTime, er.CalculatedCcyLastQty
    )
    { }

    

    public string? ClOrdId { get; init; }
    public long? OrderId { get; init; }
    public string? ExternalId { get; init; }
    public int AccountId { get; init; }
    public string? ExternalContractId { get; init; }
    public OrdStatus OrdStatus { get; init; }
    public OrdType? OrdType { get; init; }
    public Side? Side { get; init; }
    public decimal? LastQty { get; init; }
    public decimal? LastPx { get; init; }
    public decimal? OrderQty { get; init; }
    public decimal CumQty { get; init; }
    public decimal LeavesQty { get; init; }
    public decimal? Price { get; init; }
    public decimal? StopPx { get; init; }
    public TimeInForce? TimeInForce { get; init; }
    public Instant? ExpireDt { get; init; }
    public ExecType ExecType { get; init; }
    public ExecTypeReason? ExecTypeReason { get; init; }
    public RejectReason? RejectReason { get; init; }
    public string? RejectText { get; init; }
    public Instant TransactTime { get; init; }
    public decimal? CalculatedCcyLastQty { get; init; }

    public ExecutionReport ToExecutionReport(string accountServiceName, long execId, long orderId, int contractId,
        decimal? lastQtyOverride = null, decimal? lastCalculatedCcyOverride = null, ExecType? execTypeOverride = null,
        ExecTypeReason? execTypeReasonOverride = null
    ) => new (
        accountServiceName,
        orderId,
        ClOrdId,
        AccountId, 
        AccountId,
        contractId,
        null,
        null,
        OrdStatus,
        OrdType.Value,
        Side.Value,
        OrderQty.Value,
        CumQty,
        LeavesQty,
        Price,
        StopPx,
        TimeInForce.Value,
        ExpireDt,
        new Dictionary<string, LinkType>(),
        null,
        false,
        LastPx,
        lastQtyOverride ?? LastQty,
        lastCalculatedCcyOverride ?? CalculatedCcyLastQty,
        TransactTime,
        execTypeOverride ?? ExecType, 
        execTypeReasonOverride ?? ExecTypeReason,
        ExternalId,
        null,
        execId,
        null,
        RejectReason,
        RejectText,
        null
    );

    public override string ToString()
    {
        return $"{{ ExternalExecutionReport | {nameof(ClOrdId)}: {ClOrdId}, {nameof(OrderId)}: {OrderId}, {nameof(ExternalId)}: {ExternalId}, {nameof(AccountId)}: {AccountId}, {nameof(ExternalContractId)}: {ExternalContractId}, {nameof(OrdStatus)}: {OrdStatus}, {nameof(OrdType)}: {OrdType}, {nameof(Side)}: {Side}, {nameof(LastQty)}: {LastQty}, {nameof(LastPx)}: {LastPx}, {nameof(OrderQty)}: {OrderQty}, {nameof(CumQty)}: {CumQty}, {nameof(LeavesQty)}: {LeavesQty}, {nameof(Price)}: {Price}, {nameof(StopPx)}: {StopPx}, {nameof(TimeInForce)}: {TimeInForce}, {nameof(ExpireDt)}: {ExpireDt}, {nameof(ExecType)}: {ExecType}, {nameof(ExecTypeReason)}: {ExecTypeReason}, {nameof(RejectReason)}: {RejectReason}, {nameof(RejectText)}: {RejectText}, {nameof(TransactTime)}: {TransactTime}, {nameof(CalculatedCcyLastQty)}: {CalculatedCcyLastQty} }}";
    }
}