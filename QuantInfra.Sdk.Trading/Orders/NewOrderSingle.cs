using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using NodaTime;
using QuantInfra.Sdk.Utils;

namespace QuantInfra.Sdk.Trading.Orders;

public class NewOrderSingle
{
    public int AccountId { get; init; }
    public string? ClOrdId { get; init; }
    public int ContractId { get; init; }
    public string? StrategyPositionId { get; init; }
    public PositionEffect? PositionEffect { get; init; }
    public long? SignalGroupId { get; init; }
    public long? ExecutionRequestId { get; init; }
    public long? ParentPositionId { get; init; }
    
    public OrdType OrdType { get; init; }
    public Side Side { get; init; }
    public decimal OrderQty { get; init; }

    public decimal? Price { get; init; }
    public decimal? StopPx { get; init; }
        
    public TimeInForce TimeInForce { get; init; }
    public bool IsSuspended { get; init; }
    public Instant? CreatedAt { get; init; }
    public Instant? ActivationDt { get; init; }
    public Instant? ExpireDt { get; init; }

    public IReadOnlyDictionary<string, LinkType> LinkedOrders { get; init; } =
        new Dictionary<string, LinkType>();
    

    public IReadOnlyCollection<int>? TradingSessionsIds { get; init; }

    public PegInstructions? PegInstructions { get; init; }
    public bool IsSltp { get; init; }
    public IReadOnlyCollection<ExecInst> ExecInst { get; init; } = new List<ExecInst>();
    
    public IReadOnlyList<Allocation>? Allocations { get; init; }

    [JsonConstructor] internal NewOrderSingle() { }

    public NewOrderSingle(int accountId, string? clOrdId, int contractId, string? strategyPositionId,
        PositionEffect? positionEffect, long? signalGroupId, long? executionRequestId, long? parentPositionId,
        OrdType ordType, Side side, decimal orderQty, decimal? price, decimal? stopPx, TimeInForce timeInForce,
        bool isSuspended, Instant? createdAt, Instant? activationDt, Instant? expireDt,
        IReadOnlyCollection<int>? tradingSessionsIds, PegInstructions? pegInstructions, bool isSltp,
        IReadOnlyList<Allocation>? allocations, IReadOnlyDictionary<string, LinkType> linkedOrders
    )
    {
        AccountId = accountId;
        ClOrdId = clOrdId;
        ContractId = contractId;
        StrategyPositionId = strategyPositionId;
        PositionEffect = positionEffect;
        SignalGroupId = signalGroupId;
        ExecutionRequestId = executionRequestId;
        ParentPositionId = parentPositionId;
        OrdType = ordType;
        Side = side;
        OrderQty = orderQty;
        Price = price;
        StopPx = stopPx;
        TimeInForce = timeInForce;
        IsSuspended = isSuspended;
        CreatedAt = createdAt;
        ActivationDt = activationDt;
        ExpireDt = expireDt;
        TradingSessionsIds = tradingSessionsIds;
        PegInstructions = pegInstructions;
        IsSltp = isSltp;
        Allocations = allocations;
        LinkedOrders = linkedOrders.ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    public NewOrderSingle(NewOrderSingle o) : this(o.AccountId, o.ClOrdId, o.ContractId, o.StrategyPositionId,
        o.PositionEffect, o.SignalGroupId, o.ExecutionRequestId, o.ParentPositionId, o.OrdType, o.Side, o.OrderQty, o.Price, o.StopPx,
        o.TimeInForce, o.IsSuspended, o.CreatedAt, o.ActivationDt, o.ExpireDt, o.TradingSessionsIds, o.PegInstructions,
        o.IsSltp, o.Allocations, o.LinkedOrders)
    { }

    public static NewOrderSingle MarketOrder(
        string? clOrdId,
        int accountId,
        int contractId,
        string? strategyPositionId,
        PositionEffect positionEffect,
        decimal volume,
        Side side,
        long? parentPositionId = null,
        IReadOnlyDictionary<string, LinkType>? linkedOrders = null,
        Instant? expireDt = null,
        long? executionRequestId = null,
        int[]? tradingSessionsIds = null,
        ExecInst[]? execInst = null,
        bool isSuspended = false,
        Instant? activationDt = null,
        Instant? createdAt = null
    ) => new()
    {
        ClOrdId = clOrdId,
        AccountId = accountId,
        ContractId = contractId,
        StrategyPositionId = strategyPositionId,
        PositionEffect = positionEffect,
        OrdType = OrdType.Market,
        OrderQty = volume,
        Side = side,
        ParentPositionId = parentPositionId,
        LinkedOrders = 
            linkedOrders?.ToDictionary(
                o => o.Key,
                o => o.Value
            ) ?? new Dictionary<string, LinkType>(),
        TimeInForce = expireDt.HasValue ? TimeInForce.GoodTillTime : TimeInForce.GoodTillCancelled,
        ExpireDt = expireDt,
        ExecutionRequestId = executionRequestId,
        TradingSessionsIds = tradingSessionsIds != null ? new ReadOnlyCollection<int>(tradingSessionsIds) : null,
        ExecInst = execInst?.ToList() ?? new List<ExecInst>(),
        IsSuspended = isSuspended,
        ActivationDt = activationDt,
        CreatedAt = createdAt
    };

    public static NewOrderSingle LimitOrder(
        string? clOrdId,
        int accountId,
        int contractId,
        string? strategyPositionId,
        PositionEffect positionEffect,
        decimal volume,
        Side side,
        decimal price,
        long? parentPositionId = null,
        IReadOnlyDictionary<string, LinkType>? linkedOrders = null,
        bool isSuspended = false,
        bool isSLTP = false,
        Instant? createdAt = null,
        Instant? activationDt = null,
        Instant? expireDt = null,
        long? executionRequestId = null,
        int[]? tradingSessionsIds = null,
        ExecInst[]? execInst = null
    ) => new ()
    {
        ClOrdId = clOrdId,
        AccountId = accountId,
        ContractId = contractId,
        StrategyPositionId = strategyPositionId,
        PositionEffect = positionEffect,
        OrdType = OrdType.Limit,
        IsSuspended = isSuspended,
        OrderQty = volume,
        Side = side,
        Price = price,
        ParentPositionId = parentPositionId,
        LinkedOrders = linkedOrders?.ToDictionary(
            o => o.Key,
            o => o.Value
        ) ?? new Dictionary<string, LinkType>(),
        IsSltp = isSLTP,
        TimeInForce = expireDt.HasValue ? TimeInForce.GoodTillTime : TimeInForce.GoodTillCancelled,
        CreatedAt = createdAt,
        ActivationDt = activationDt,
        ExpireDt = expireDt,
        ExecutionRequestId = executionRequestId,
        TradingSessionsIds = tradingSessionsIds != null ? new ReadOnlyCollection<int>(tradingSessionsIds) : null,
        ExecInst = execInst?.ToList() ?? new List<ExecInst>(),
    };

    public static NewOrderSingle StopMarketOrder(
        string? clOrdId,
        int accountId,
        int contractId,
        string? strategyPositionId,
        PositionEffect positionEffect,
        decimal volume,
        Side side,
        decimal stopPx,
        long? parentPositionId = null,
        IReadOnlyDictionary<string, LinkType>? linkedOrders = null,
        bool isSuspended = false,
        Instant? activationDt = null,
        bool isSLTP = false,
        PegInstructions? pegInstructions = null,
        Instant? createdAt = null,
        Instant? expireDt = null,
        long? executionRequestId = null,
        int[]? tradingSessionsIds = null,
        ExecInst[]? execInst = null
    ) => new()
    {
        ClOrdId = clOrdId,
        AccountId = accountId,
        ContractId = contractId,
        StrategyPositionId = strategyPositionId,
        PositionEffect = positionEffect,
        OrdType = OrdType.StopMarket,
        IsSuspended = isSuspended,
        OrderQty = volume,
        Side = side,
        StopPx = stopPx,
        ParentPositionId = parentPositionId,
        LinkedOrders = linkedOrders?.ToDictionary(
            o => o.Key,
            o => o.Value
        ) ?? new Dictionary<string, LinkType>(),
        IsSltp = isSLTP,
        PegInstructions = pegInstructions,
        TimeInForce = expireDt.HasValue ? TimeInForce.GoodTillTime : TimeInForce.GoodTillCancelled,
        CreatedAt = createdAt,
        ActivationDt = activationDt,
        ExpireDt = expireDt,
        ExecutionRequestId = executionRequestId,
        TradingSessionsIds = tradingSessionsIds != null ? new ReadOnlyCollection<int>(tradingSessionsIds) : null,
        ExecInst = execInst?.ToList() ?? new List<ExecInst>(),
    };

    public static NewOrderSingle StopLimitOrder(
        string? clOrdId,
        int accountId,
        int contractId,
        string? strategyPositionId,
        PositionEffect positionEffect,
        decimal volume,
        Side side,
        decimal price,
        decimal stopPx,
        long? parentPositionId = null,
        IReadOnlyDictionary<string, LinkType>? linkedOrders = null,
        bool isSuspended = false,
        Instant? activationDt = null,
        Instant? createdAt = null,
        Instant? expireDt = null,
        long? executionRequestId = null,
        int[]? tradingSessionsIds = null,
        ExecInst[]? execInst = null
    ) => new ()
    {
        ClOrdId = clOrdId,
        AccountId = accountId,
        ContractId = contractId,
        StrategyPositionId = strategyPositionId,
        PositionEffect = positionEffect,
        OrdType = OrdType.StopLimit,
        IsSuspended = isSuspended,
        OrderQty = volume,
        Side = side,
        Price = price,
        StopPx = stopPx,
        ParentPositionId = parentPositionId,
        LinkedOrders = linkedOrders?.ToDictionary(
            o => o.Key,
            o => o.Value
        ) ?? new Dictionary<string, LinkType>(),
        TimeInForce = expireDt.HasValue ? TimeInForce.GoodTillTime : TimeInForce.GoodTillCancelled,
        CreatedAt = createdAt,
        ActivationDt = activationDt,
        ExpireDt = expireDt,
        ExecutionRequestId = executionRequestId,
        TradingSessionsIds = tradingSessionsIds != null ? new ReadOnlyCollection<int>(tradingSessionsIds) : null,
        ExecInst = execInst?.ToList() ?? new List<ExecInst>(),
    };

    public static NewOrderSingle MarketIfTouched(
        string? clOrdId,
        int accountId,
        int contractId,
        string? strategyPositionId,
        PositionEffect positionEffect,
        decimal volume,
        Side side,
        decimal price,
        long? parentPositionId = null,
        IReadOnlyDictionary<string, LinkType>? linkedOrders = null,
        bool isSuspended = false,
        Instant? createdAt = null,
        Instant? activationDt = null,
        Instant? expireDt = null,
        long? executionRequestId = null,
        int[]? tradingSessionsIds = null,
        ExecInst[]? execInst = null
    ) => new ()
    {
        ClOrdId = clOrdId,
        AccountId = accountId,
        ContractId = contractId,
        StrategyPositionId = strategyPositionId,
        PositionEffect = positionEffect,
        OrdType = OrdType.MarketIfTouched,
        IsSuspended = isSuspended,
        OrderQty = volume,
        Side = side,
        Price = price,
        ParentPositionId = parentPositionId,
        LinkedOrders = linkedOrders?.ToDictionary(
            o => o.Key,
            o => o.Value
        ) ?? new Dictionary<string, LinkType>(),
        TimeInForce = expireDt.HasValue ? TimeInForce.GoodTillTime : TimeInForce.GoodTillCancelled,
        CreatedAt = createdAt,
        ActivationDt = activationDt,
        ExpireDt = expireDt,
        ExecutionRequestId = executionRequestId,
        TradingSessionsIds = tradingSessionsIds != null ? new ReadOnlyCollection<int>(tradingSessionsIds) : null,
        ExecInst = execInst?.ToList() ?? new List<ExecInst>(),
    };

    public static NewOrderSingle TakeProfit(int accountId,
        int contractId,
        decimal volume,
        Side side,
        decimal price,
        bool positionAlreadyOpened = true,
        string? strategyPositionId = null,
        string? clOrdId = null,
        int[]? tradingSessionIds = null
    ) => LimitOrder(
        clOrdId ?? $"{strategyPositionId}-tp",
        accountId,
        contractId,
        strategyPositionId,
        Orders.PositionEffect.Close,
        volume,
        side,
        price, 
        isSuspended: !positionAlreadyOpened,
        isSLTP: true, 
        tradingSessionsIds: tradingSessionIds
    );
    
    public static NewOrderSingle StopLoss(int accountId,
        int contractId,
        decimal volume,
        Side side,
        decimal stopPx,
        bool positionAlreadyOpened = true,
        string? strategyPositionId = null,
        string? clOrdId = null,
        int[]? tradingSessionIds = null
    ) => StopMarketOrder(
        clOrdId ?? $"{strategyPositionId}-sl",
        accountId,
        contractId,
        strategyPositionId,
        Orders.PositionEffect.Close,
        volume,
        side,
        stopPx,
        isSuspended: !positionAlreadyOpened,
        isSLTP: true, tradingSessionsIds: tradingSessionIds);
    

    protected virtual bool Equals(Order other) =>
        ClOrdId == other.ClOrdId
        && AccountId == other.AccountId
        && ContractId == other.ContractId
        && Nullable.Equals(StrategyPositionId, other.StrategyPositionId) 
        && Nullable.Equals(SignalGroupId, other.SignalGroupId) 
        && PositionEffect == other.PositionEffect 
        && Nullable.Equals(ExecutionRequestId, other.ExecutionRequestId)
        && OrdType == other.OrdType 
        && Side == other.Side 
        && OrderQty == other.OrderQty 
        && Price == other.Price 
        && StopPx == other.StopPx 
        && ParentPositionId == other.ParentPositionId
        && TimeInForce == other.TimeInForce 
        && Nullable.Equals(ExpireDt, other.ExpireDt)
        && !LinkedOrders.Keys.Except(other.LinkedOrders.Keys).Any()
        && !other.LinkedOrders.Keys.Except(LinkedOrders.Keys).Any()
        && LinkedOrders.All(kv => kv.Value == other.LinkedOrders[kv.Key])
        && IsSuspended == other.IsSuspended 
        && (
            (TradingSessionsIds == null && other.TradingSessionsIds == null)
            || (
                TradingSessionsIds != null 
                && other.TradingSessionsIds != null 
                && !TradingSessionsIds.Except(other.TradingSessionsIds).Any()
                && !other.TradingSessionsIds.Except(TradingSessionsIds).Any()
            )
        ) 
        && Nullable.Equals(PegInstructions, other.PegInstructions) 
        && IsSltp == other.IsSltp
        && (
            (ExecInst == null && other.ExecInst == null)
            || (
                ExecInst != null 
                && other.ExecInst != null 
                && !ExecInst.Except(other.ExecInst).Any()
                && !other.ExecInst.Except(ExecInst).Any()
            )
        );

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        // if (obj.GetType() != this.GetType()) return false;
        if (obj is not NewOrderSingle o) return false;
        return Equals(o);
    }
    
    public static NewOrderSingle[] OneCancelsAnother(params NewOrderSingle[] orders)
    {
        var clOrdIds = orders
            .Select(o => o.ClOrdId ?? throw new InvalidOperationException("ClOrdId must be provided for all orders when using OCA"))
            .ToHashSet();
        var res = orders.Select(o => new NewOrderSingle(o)
        {
            LinkedOrders = clOrdIds.Where(x => x != o.ClOrdId).ToDictionary(x => x, x => LinkType.OneCancelsOther) 
        }).ToArray();
        return res;
    }
    
    protected virtual string GetFormattingString() =>
        $"ClOrdId: {ClOrdId}, AccountId: {AccountId}, ContractId: {ContractId}, StrategyPositionId: {StrategyPositionId}, PositionEffect: {PositionEffect}, ExecutionRequestId: {ExecutionRequestId}, OrdType: {OrdType}, Side: {Side}, OrderQty: {OrderQty}, Price: {Price}, StopPx: {StopPx}, TimeInForce: {TimeInForce}, ExpireDt: {ExpireDt}, LinkedOrders: [{LinkedOrders.ToFormattedString()}], IsSuspended: {IsSuspended}, PegInstructions: {PegInstructions}, IsSLTP: {IsSltp}, ExecInst: [{ExecInst.ToFormattedString()}], Allocations: [{Allocations.ToFormattedString()}]]";

    public override string ToString() =>
        $"{{ NewOrderSingle | {GetFormattingString()} }}";
}