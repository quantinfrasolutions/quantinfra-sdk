using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Common.Trading;
using NodaTime;
using QuantInfra.Sdk.Trading.ExternalAccounts;

[assembly: InternalsVisibleTo("Databases.Main")]
namespace QuantInfra.Sdk.Trading.Orders;

public class OrderStatus : Order
{
    public OrdStatus OrdStatus { get; init; }
    public decimal CumQty { get; init; }
    public decimal LeavesQty { get; init; }
    public Instant TransactTime { get; init; }
    public RejectReason? RejectReason { get; init; }
    public string? RejectText { get; init; }

    [JsonConstructor] internal OrderStatus() { }

    public OrderStatus(NewOrderSingle o, string accountServiceName, long orderId, OrdStatus ordStatus, decimal cumQty, decimal leavesQty, RejectReason? rejectReason,
        string? rejectText) : base(o, accountServiceName, orderId)
    {
        OrdStatus = ordStatus;
        CumQty = cumQty;
        LeavesQty = leavesQty;
        RejectReason = rejectReason;
        RejectText = rejectText;
    }

    public OrderStatus(Order o, Instant transactTime) : this(o, OrdStatus.PendingNew, 0, o.OrderQty, transactTime,
        null, null)
    {
    }

    public OrderStatus(OrderStatus o) : base(o)
    {
        OrdStatus = o.OrdStatus;
        CumQty = o.CumQty;
        LeavesQty = o.LeavesQty;
        TransactTime = o.TransactTime;
        RejectReason = o.RejectReason;
        RejectText = o.RejectText;
    }

    public OrderStatus(Order o, OrdStatus ordStatus, decimal cumQty, decimal leavesQty, Instant transactTime,
        RejectReason? rejectReason, string? rejectText) : base(o)
    {
        OrdStatus = ordStatus;
        CumQty = cumQty;
        LeavesQty = leavesQty;
        TransactTime = transactTime;
        RejectReason = rejectReason;
        RejectText = rejectText;
    }

    public OrderStatus(string accountServiceName, long orderId, string? clOrdId, int accountId, int? brokerAccountId, int contractId, string strategyPositionId,
        PositionEffect? positionEffect,
        OrdStatus ordStatus, OrdType ordType, Side side, decimal orderQty, decimal cumQty, decimal leavesQty,
        decimal? price, decimal? stopPx,
        TimeInForce timeInForce, Instant? expireDt, IReadOnlyDictionary<string, LinkType> linkedOrders,
        PegInstructions? pegInstructions, bool isSltp, string? externalId, long? executionRequestId,
        long? signalGroupId,
        RejectReason? rejectReason, string? rejectText
    )
    {
        AccountServiceName = accountServiceName;
        OrderId = orderId;
        ClOrdId = clOrdId;
        AccountId = accountId;
        BrokerAccountId = brokerAccountId;
        ContractId = contractId;
        StrategyPositionId = strategyPositionId;
        PositionEffect = positionEffect;
        OrdStatus = ordStatus;
        OrdType = ordType;
        Side = side;
        OrderQty = orderQty;
        CumQty = cumQty;
        LeavesQty = leavesQty;
        Price = price;
        StopPx = stopPx;
        TimeInForce = timeInForce;
        ExpireDt = expireDt;
        LinkedOrders = linkedOrders;
        PegInstructions = pegInstructions;
        IsSltp = isSltp;
        ExternalId = externalId;
        ExecutionRequestId = executionRequestId;
        SignalGroupId = signalGroupId;
        RejectReason = rejectReason;
        RejectText = rejectText;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="price">The price against which to check</param>
    /// <param name="useStopPx">Only for testing using aggregated bars: in this case stop orders are executed by their stopPx</param>
    /// <param name="executionPrice">May to be provided for stop orders when useStopPx == false and execution price needs to differ from the trigger price</param>
    /// <returns></returns>
    // ReSharper disable once InvalidXmlDocComment
    public ExecutionReport? CheckExecution(IExecIdProvider execIdProvider, decimal price, IContract contract, Instant referenceDt,
        Instant processingDt, IPnLCalculator plCalculator,
        bool useStopPx = false, decimal? executionPrice = null, int? tradingSessionId = null,
        bool immediateCancellation = false
    )
    {
        // If by any chance we have an order that is placed later then it is being checked, do not execute
        if (referenceDt < TransactTime || processingDt < TransactTime) return null;

        if (OrdStatus == OrdStatus.PendingCancel)
            return ConfirmCancellation(execIdProvider, referenceDt);

        if (OrdStatus.IsTerminal() || OrdStatus == OrdStatus.Suspended)
            return null;

        if (TimeInForce == TimeInForce.GoodTillTime && processingDt > ExpireDt)
            return CancelOrder(execIdProvider, processingDt, immediateCancellation: immediateCancellation);

        if (TradingSessionsIds != null &&
            (tradingSessionId == null || !TradingSessionsIds.Contains(tradingSessionId.Value))
           ) return null;

        // Market orders are executed by the last tick
        if (OrdType == OrdType.Market)
            return Execute(execIdProvider.GetNextExecId(), executionPrice ?? price, LeavesQty, referenceDt, plCalculator);

        // Limit orders are executed by their Price
        if (OrdType == OrdType.Limit)
        {
            if (
                (Side == Side.Buy && price <= Price)
                || (Side == Side.Sell && price >= Price)
            )
            {
                return Execute(execIdProvider.GetNextExecId(), Price!.Value, LeavesQty, referenceDt, plCalculator);
            }

            return null;
        }

        // Stop orders are executed by the close price, except when the respective flag is set. This is used for
        // backtesting on aggregated bars.
        // For stop market orders, this will return Fill in case of execution.
        if (OrdType == OrdType.StopMarket || OrdType == OrdType.StopLimit)
        {
            var er = MovePeggedOrder(price, contract, execIdProvider, referenceDt);
            var stopPx = er?.StopPx ?? StopPx!.Value;

            if (
                (Side == Side.Buy && price >= stopPx)
                || (Side == Side.Sell && price <= stopPx)
            )
            {
                return TriggerStopOrder(execIdProvider, useStopPx ? stopPx : (executionPrice ?? price), referenceDt, plCalculator);
            }

            return er;
        }

        // MIT orders are executed by price
        if (OrdType == OrdType.MarketIfTouched)
        {
            if (
                (Side == Side.Buy && price <= Price)
                || (Side == Side.Sell && price >= Price)
            )
            {
                return Execute(execIdProvider.GetNextExecId(), price, LeavesQty, referenceDt, plCalculator);
            }
        }

        return null;
    }

    public ExecutionReport GetStatus(Instant ts) => new ExecutionReport(this, ExecType.OrderStatus, 0, ts);

    public ExecutionReport? CancelOrder(
        IExecIdProvider execIdProvider,
        Instant ts,
        RejectReason? rejectReason = null,
        bool immediateCancellation = false,
        string? requestId = null
    )
    {
        if (OrdStatus.IsTerminal())
            return null;

        return new ExecutionReport(
            this,
            immediateCancellation ? ExecType.Canceled : ExecType.PendingCancel,
            execIdProvider.GetNextExecId(),
            ts
        )
        {
            OrdStatus = immediateCancellation ? OrdStatus.Canceled : OrdStatus.PendingCancel,
            ExecTypeReason = immediateCancellation ? null : ExecTypeReason.OrderChangeInitiated,
            RejectReason = rejectReason,
            RequestId = requestId,
        };
    }

    public ExecutionReport CancelOrder(long execId, Instant ts) =>
        new ExecutionReport(
            this,
            ExecType.PendingCancel,
            execId, 
            ts
        )
        {
            OrdStatus = OrdStatus.PendingCancel,
        };

    public ExecutionReport? ConfirmCancellation(IExecIdProvider execIdProvider, Instant ts) =>
        OrdStatus == OrdStatus.PendingCancel
            ? new ExecutionReport(this, ExecType.Canceled, execIdProvider.GetNextExecId(), ts)
            {
                OrdStatus = OrdStatus.Canceled,
                LeavesQty = 0,
            }
            : null;

    public ExecutionReport RejectCancellation(long execId, Instant ts, string? rejectText, OrdStatus? previousStatus) =>
        new (this,  ExecType.OrderStatus, execId, ts)
        {
            OrdStatus = previousStatus.HasValue
                ? previousStatus.Value
                : OrdStatus != OrdStatus.PendingCancel
                    ? OrdStatus
                    : CumQty == 0 ? OrdStatus.New : OrdStatus.PartiallyFilled,
            ExecTypeReason = ExecTypeReason.PendingCxlExecuted,
            RejectText = rejectText,
        };

    public ExecutionReport? RequestReplace(IExecIdProvider execIdProvider, OrderReplaceRequest request, Instant ts)
    {
        if (OrdStatus.IsTerminal()) return null;
            
        return new ExecutionReport(this, ExecType.PendingReplace, execIdProvider.GetNextExecId(), ts)
        {
            OrderQty = request.OrderQty ?? OrderQty,
            Price = request.Price ?? Price,
            StopPx = request.StopPx ?? StopPx,
            ExecTypeReason = ExecTypeReason.OrderChangeInitiated,
            OrdStatus = OrdStatus.PendingReplace,
            RequestId = request.RequestId,
        };
    }
        
    public ExecutionReport RequestReplace(long execId, OrderReplaceRequest request, Instant ts) =>
        new ExecutionReport(this, ExecType.PendingReplace, 0, ts)
        {
            OrderQty = request.OrderQty ?? OrderQty,
            Price = request.Price ?? Price,
            StopPx = request.StopPx ?? StopPx,
            OrdStatus = OrdStatus.PendingReplace,
            RequestId = request.RequestId,
        };

    public ExecutionReport ConfirmReplace(long execId, OrderReplaceRequest request, Instant ts)
    {
        if (OrdStatus.IsTerminal())
            return RejectOrder(execId, Orders.RejectReason.TooLateToCancel, $"Order is in {OrdStatus} state", ts);

        if (request.OrderQty.HasValue)
        {
            var newOrderQty = request.OrderQty.Value;
            if (newOrderQty <= CumQty)
            {
                return new ExecutionReport(this, ExecType.Replaced, execId, ts)
                {
                    OrderQty = newOrderQty,
                    LeavesQty = 0,
                    OrdStatus = OrdStatus.Filled,
                    TransactTime = ts,
                    RequestId = request.RequestId,
                };
            }
        }
            
        return new ExecutionReport(this, ExecType.Replaced, execId, ts)
        {
            OrderQty = request.OrderQty ?? OrderQty,
            Price = request.Price ?? Price,
            StopPx = request.StopPx ?? StopPx,
        };
    }
        
    public ExecutionReport? ConfirmReplace(long execId, Instant ts)
    {
        if (OrdStatus != OrdStatus.PendingReplace) return null;
        return new ExecutionReport(this, ExecType.Replaced, execId, ts);
    }

    public ExecutionReport Execute(long execId, decimal price, decimal volume, Instant executionTime, IPnLCalculator plCalculator)
    {
        if (volume <= 0) throw new ArgumentException("Volume must be positive");

        var leavesQty = Math.Max(0, LeavesQty - volume);
        return new ExecutionReport(this, ExecType.Fill, execId, executionTime)
        {
            LastPx = price,
            LastQty = volume,
            CumQty = CumQty + volume,
            LeavesQty = leavesQty,
            OrdStatus = leavesQty > 0 ? OrdStatus.PartiallyFilled : OrdStatus.Filled,
            CalculatedCcyLastQty = plCalculator.GetValueInSettlementCcy(price, volume),
        };
    }

    private ExecutionReport? MovePeggedOrder(decimal price, IContract contract, IExecIdProvider execIdProvider, Instant executionTime)
    {
        if (OrdStatus.IsTerminal())
            return null;

        if (OrdType == OrdType.StopMarket && PegInstructions != null)
        {
            decimal desiredLevel = contract.NormalizePrice(PegInstructions!.Value.PegMoveType switch
            {
                PegMoveType.Floating => price *
                                        (1 + (Side == Side.Buy ? -1 : 1) * PegInstructions!.Value.PegOffsetValue),
                PegMoveType.Fixed => price + (Side == Side.Buy ? -1 : 1) * PegInstructions!.Value.PegOffsetValue,
                PegMoveType.Approaching => StopPx!.Value +
                                           (Side == Side.Buy ? -1 : 1) * price *
                                           PegInstructions!.Value.PegOffsetValue,
                _ => throw new NotSupportedException(),
            });

            desiredLevel = Side == Side.Buy
                ? Math.Min(desiredLevel, StopPx!.Value)
                : Math.Max(desiredLevel, StopPx!.Value);

            if (desiredLevel != StopPx)
            {
                return RequestReplace(execIdProvider, new OrderReplaceRequest { StopPx = desiredLevel }, executionTime);
            }
        }

        return null;
    }

    private ExecutionReport? TriggerStopOrder(IExecIdProvider execIdProvider, decimal price, Instant executionTime, IPnLCalculator plCalculator)
    {
        if (OrdStatus.IsTerminal())
            return null;

        if (OrdType == OrdType.StopMarket)
        {
            return Execute(execIdProvider.GetNextExecId(), price, LeavesQty, executionTime, plCalculator);
        }

        if (OrdType == OrdType.StopLimit)
        {
            return new ExecutionReport(this, ExecType.New, execIdProvider.GetNextExecId(), executionTime)
            {
                OrdType = OrdType.Limit,
                StopPx = null,
                OrdStatus = OrdStatus.New,
                ExecTypeReason = ExecTypeReason.SuspendedOrderActivated,
            };
        }

        return null;
    }

    public ExecutionReport? CheckPendingOrderActivation(IExecIdProvider execIdProvider, decimal price, Instant dt)
    {
        if (OrdStatus.IsTerminal()) return null;
            
        var activated = (
            (OrdType == OrdType.StopMarket || OrdType == OrdType.StopLimit)
            && ((Side == Side.Buy && price >= StopPx) || (Side == Side.Sell && price <= StopPx))
        ) || (
            OrdType == OrdType.MarketIfTouched 
            && ((Side == Side.Buy && price <= StopPx) || (Side == Side.Sell && price >= StopPx))  
        );
            
        if (!activated) return null;

        return new ExecutionReport(this, ExecType.TriggeredOrActivatedBySystem, execIdProvider.GetNextExecId(), dt)
        {
            OrdType = OrdType switch
            {
                OrdType.StopMarket => OrdType.Market,
                OrdType.MarketIfTouched => OrdType.Market,
                OrdType.StopLimit => OrdType.Limit,
                _ => throw new InvalidOperationException($"Tried to activate an order with type {OrdType}")
            },
            StopPx = null,
            OrdStatus = OrdStatus.PendingNew,
            ExecTypeReason = ExecTypeReason.SuspendedOrderActivated,
            IsVirtual = false,
        };
    }

    public ExecutionReport? ActivateSuspendedOrder(IExecIdProvider execIdProvider, Instant executionTime)
    {
        if (OrdStatus != OrdStatus.Suspended)
            return null;

        return new ExecutionReport(this, ExecType.TriggeredOrActivatedBySystem, execIdProvider.GetNextExecId(), executionTime)
        {
            OrdStatus = OrdStatus.PendingNew,
            ExecTypeReason = ExecTypeReason.SuspendedOrderActivated,
            IsSuspended = false
        };
    }

    public ExecutionReport ApplyExternalExecutionReport(ExternalExecutionReport er, long execId, int contractId, ExecType? execTypeOverride = null, ExecTypeReason? execTypeReasonOverride = null) =>
        new ExecutionReport(this, execTypeOverride ?? er.ExecType, execId, er.TransactTime)
        {
            ContractId = contractId,
            OrdType = er.OrdType ?? OrdType,
            OrderQty = er.OrderQty ?? OrderQty,
            Side = er.Side ?? Side,
            Price = er.Price ?? Price,
            StopPx = er.StopPx ?? StopPx,
            OrdStatus = er.OrdStatus,
            ExecTypeReason = execTypeReasonOverride ?? er.ExecTypeReason,
            ExternalId = er.ExternalId,
            CumQty = er.CumQty,
            LeavesQty = er.LeavesQty,
            LastQty = er.LastQty ?? er.CumQty - CumQty,
            RejectReason = er.RejectReason,
            RejectText = er.RejectText
        };
        
    public ExecutionReport AssignToBrokerAccount(long execId, int brokerAccountId, Instant transactTime)
    {
        return new(this, ExecType.PendingNew, execId, transactTime)
        {
            BrokerAccountId = brokerAccountId,
            OrdStatus = OrdStatus,
            ExecTypeReason = ExecTypeReason.BrokerAccountAssignedToOrder,
            TransactTime = transactTime
        };
    }


    public new string GetFormattingString() =>
        $"{base.GetFormattingString()}, OrdStatus: {OrdStatus}, CumQty: {CumQty}, LeavesQty: {LeavesQty}, TransactTime: {TransactTime}, RejectReason={RejectReason}, RejectText={RejectText}";

    public override string ToString() =>
        $"{{ OrderStatus | {GetFormattingString()} }}";

    protected bool Equals(OrderStatus other)
    {
        return base.Equals(other) && OrdStatus == other.OrdStatus && CumQty == other.CumQty && LeavesQty == other.LeavesQty /*&& TransactTime.Equals(other.TransactTime)*/ && RejectReason == other.RejectReason && RejectText == other.RejectText;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not OrderStatus os) return false;
        return Equals(os);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)OrdStatus, CumQty, LeavesQty, TransactTime, RejectReason, RejectText);
    }
}