using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using Common.Trading;
using NodaTime;

[assembly:InternalsVisibleTo("System.Text.Json")]
namespace QuantInfra.Sdk.Trading.Orders;

public class ExecutionReport : OrderStatus
{
    internal ExecutionReport(OrderStatus o, ExecType execType, long execId, Instant ts) : base(o)
    {
        ExecType = execType;
        ExecId = execId;
        TransactTime = ts;
    }
        
    [JsonConstructor]
    public ExecutionReport(
        string accountServiceName,
        long orderId,
        string? clOrdId,
        int accountId,
        int? brokerAccountId,
        int contractId,
        string strategyPositionId,
        PositionEffect? positionEffect, 
        OrdStatus ordStatus,
        OrdType ordType,
        Side side,
        decimal orderQty,
        decimal cumQty,
        decimal leavesQty,
        decimal? price, 
        decimal? stopPx,
        TimeInForce timeInForce,
        Instant? expireDt,
        IReadOnlyDictionary<string, LinkType> linkedOrders,
        PegInstructions? pegInstructions, 
        bool isSltp,
        decimal? lastPx,
        decimal? lastQty,
        decimal? calculatedCcyLastQty,
        Instant transactTime,
        ExecType execType,
        ExecTypeReason? execTypeReason,
        string? externalId,
        long? executionRequestId,
        long execId,
        long? signalGroupId,
        RejectReason? rejectReason,
        string? rejectText,
        string? requestId
    ) : base(accountServiceName, orderId, clOrdId, accountId, brokerAccountId, contractId, strategyPositionId, positionEffect, ordStatus, ordType, side, orderQty, cumQty, 
        leavesQty, price, stopPx, timeInForce, expireDt, linkedOrders, pegInstructions, isSltp, externalId, executionRequestId, signalGroupId, rejectReason, rejectText)
    {
        LastPx = lastPx;
        LastQty = lastQty;
        CalculatedCcyLastQty = calculatedCcyLastQty;
        TransactTime = transactTime;
        ExecType = execType;
        ExecTypeReason = execTypeReason;
        ExecId = execId;
        RequestId = requestId;
    }

    public ExecutionReport(ExecutionReport er) : base(er, er.OrdStatus, er.CumQty, er.LeavesQty, er.TransactTime,
        er.RejectReason, er.RejectText)
    {
        LastPx = er.LastPx;
        LastQty = er.LastQty;
        CalculatedCcyLastQty = er.CalculatedCcyLastQty;
        TransactTime = er.TransactTime;
        ExecType = er.ExecType;
        ExecTypeReason = er.ExecTypeReason;
        ExecId = er.ExecId;
        RequestId = er.RequestId;
    }

    public ExecutionReport() { }

    public decimal? LastPx { get; internal init; }
    public decimal? LastQty { get; internal init; }
    public decimal? CalculatedCcyLastQty { get; internal init; }
    public ExecType ExecType { get; internal init; }
    public ExecTypeReason? ExecTypeReason { get; internal init; }
    public string? RequestId { get; internal init; }

    public decimal? SignedLastQty => LastQty * Side.GetSign();
        
    public long ExecId { get; init; }

    public Trade? FillToTrade(long tradeId, decimal commission, Dictionary<int, decimal> commissions,
        int paymentCcyId, decimal fxRate, bool isSynthetic)
    {
        if (ExecType != ExecType.Fill)
            return null;

        return new Trade(
            AccountServiceName,
            tradeId,
            ClOrdId,
            AccountId,
            ContractId,
            OrderId, 
            ExecId,
            StrategyPositionId, 
            SignalGroupId,
            PositionEffect,
            Side,
            LastQty!.Value,
            LastPx!.Value,
            commission,
            TransactTime,
            ExecutionRequestId,
            null,
            commissions,
            paymentCcyId, 
            fxRate,
            CalculatedCcyLastQty ?? 0m, 
            ParentPositionId, 
            null,
            isSynthetic
        ) { ExecId = ExecId };
    }

    protected new string GetFormattingString() =>
        $"{base.GetFormattingString()}, ExecId: {ExecId}, LastPx: {LastPx}, LastQty: {LastQty}, ExecType: {ExecType}, ExecTypeReason: {ExecTypeReason}, SignedLastQty: {SignedLastQty}";

    public override string ToString() =>
        $"{{ ExecutionReport | {GetFormattingString()} }}";

    protected bool Equals(ExecutionReport other)
    {
        return base.Equals(other) && LastPx == other.LastPx && LastQty == other.LastQty && CalculatedCcyLastQty == other.CalculatedCcyLastQty && ExecType == other.ExecType && ExecTypeReason == other.ExecTypeReason && RequestId == other.RequestId && ExecId == other.ExecId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not ExecutionReport er) return false;
        return Equals(er);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LastPx, LastQty, CalculatedCcyLastQty, (int)ExecType, ExecTypeReason, RequestId, ExecId);
    }

    public static ExecutionReport OutrightReject(NewOrderSingle order, string accountServiceName, long execId, RejectReason rejectReason, string? rejectText,
        Instant processingDt) => new(
        accountServiceName,
        0,
        order.ClOrdId,
        order.AccountId,
        null,
        order.ContractId,
        order.StrategyPositionId,
        order.PositionEffect,
        OrdStatus.Rejected,
        order.OrdType,
        order.Side,
        order.OrderQty,
        0,
        0,
        order.Price,
        order.StopPx,
        order.TimeInForce,
        order.ExpireDt,
        order.LinkedOrders,
        order.PegInstructions,
        order.IsSltp,
        null,
        null,
        null,
        processingDt,
        ExecType.Rejected,
        null,
        null,
        order.ExecutionRequestId,
        execId,
        order.SignalGroupId,
        rejectReason,
        rejectText,
        null
    );

    public string GetLogString()
    {
        var sb = new StringBuilder();
        sb.Append($"execId={ExecId}, orderId={OrderId}, contractId={ContractId}, transactTime={TransactTime}, execType={ExecType}, ordStatus={OrdStatus}");
        switch (ExecType)
        {
            case ExecType.PendingNew:
                sb.Append($", ordType={OrdType}, qty={OrderQty}, side={Side}");
                break;
            case ExecType.Fill:
                sb.Append($", lastQty={LastQty}, lastPx={LastPx}, leavesQty={LeavesQty}");
                break;
        }
        return sb.ToString();
    }
}