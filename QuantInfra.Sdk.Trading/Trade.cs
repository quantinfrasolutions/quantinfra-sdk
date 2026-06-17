using System;
using System.Collections.Generic;
using NodaTime;
using QuantInfra.Sdk.Trading.Orders;

namespace Common.Trading
{
    public class Trade
    {
        public Trade() { }

        public Trade(Trade t) : this(t.AccountServiceName, t.TradeId, t.ClOrdId, t.AccountId, t.ContractId,
            t.OrderId, t.ExecId, t.StrategyPositionId, t.SignalGroupId, t.PositionEffect, t.Side, t.Volume, t.Price, t.Commission,
            t.Dt, t.ExecutionRequestId, t.ExternalTradeId, t.Commissions, t.PaymentCurrencyId, t.FxRate, t.CalculatedCcyLastQty,
            t.ParentPositionId, t.OrigTradeId, t.IsSynthetic)
        {}

        public Trade(string accountServiceName, long tradeId, string? clOrdId, int accountId, int contractId, long? orderId,
            long? execId, string? strategyPositionId, long? signalGroupId, PositionEffect? positionEffect, Side side, decimal volume,
            decimal price, decimal commission, Instant dt, long? executionRequestId,
            string? externalTradeId, IReadOnlyDictionary<int, decimal>? commissions, int paymentCurrencyId, decimal fxRate, 
            decimal calculatedCcyLastQty, long? parentPositionId, long? origTradeId, bool isSynthetic
        )
        {
            AccountServiceName = accountServiceName;
            TradeId = tradeId;
            OrigTradeId = origTradeId;
            ClOrdId = clOrdId;
            AccountId = accountId;
            ContractId = contractId;
            OrderId = orderId;
            ExecId = execId;
            StrategyPositionId = strategyPositionId;
            SignalGroupId = signalGroupId;
            PositionEffect = positionEffect;
            Side = side;
            Volume = volume;
            Price = price;
            Commission = commission;
            Dt = dt;
            ExecutionRequestId = executionRequestId;
            ExternalTradeId = externalTradeId;
            Commissions = commissions ?? new Dictionary<int, decimal>();
            PaymentCurrencyId = paymentCurrencyId;
            FxRate = fxRate;
            CalculatedCcyLastQty = calculatedCcyLastQty;
            ParentPositionId = parentPositionId;
            IsSynthetic = isSynthetic;
        }

        public decimal SignedVolume => Volume * Side.GetSign();
        public string AccountServiceName { get; init; } // ???
        public long TradeId { get; init; }
        public long? OrigTradeId { get; init; }
        public string? ClOrdId { get; init; }
        public int AccountId { get; init; }
        public int ContractId { get; init; }
        public long? OrderId { get; init; }
        public long? ExecId { get; init; }
        public string? StrategyPositionId { get; init; }
        public long? SignalGroupId { get; init; }
        public PositionEffect? PositionEffect { get; init; }
        public Side Side { get; init; }
        public decimal Volume { get; init; }
        public decimal Price { get; init; }
        public decimal Commission { get; init; }
        public Instant Dt { get; init; }
        public long? ExecutionRequestId { get; set; }
        public string? ExternalTradeId { get; init; }
        public IReadOnlyDictionary<int, decimal> Commissions { get; init; }
        public int PaymentCurrencyId { get; init; }
        public decimal FxRate { get; init; }
        public decimal CalculatedCcyLastQty { get; init; }
        public long? ParentPositionId { get; init; }
        public bool IsSynthetic { get; init; }

        public Trade CreateAllocation(long tradeId, int accountId) => new Trade(this)
        {
            TradeId = tradeId,
            AccountId = accountId,
            OrigTradeId = TradeId
        };

        public override bool Equals(object obj) => 
            obj != null
            && obj is Trade t
            && TradeId == t.TradeId
            && ClOrdId == t.ClOrdId
            && AccountId == t.AccountId
            && ContractId == t.ContractId
            && OrderId == t.OrderId
            && StrategyPositionId == t.StrategyPositionId
            && SignalGroupId == t.SignalGroupId
            && PositionEffect == t.PositionEffect
            && Side == t.Side
            && Volume == t.Volume
            && Price == t.Price
            && Commission == t.Commission
            && Dt == t.Dt
            && ExecutionRequestId == t.ExecutionRequestId
            && ExternalTradeId == t.ExternalTradeId
            && PaymentCurrencyId == t.PaymentCurrencyId
            && FxRate == t.FxRate
            && CalculatedCcyLastQty == t.CalculatedCcyLastQty
            && ParentPositionId == t.ParentPositionId
            && IsSynthetic == t.IsSynthetic;

        public override string ToString()
        {
            return
                $"{{ Trade | {nameof(SignedVolume)}: {SignedVolume}, {nameof(AccountServiceName)}: {AccountServiceName}, {nameof(TradeId)}: {TradeId}, {nameof(OrigTradeId)}: {OrigTradeId}, {nameof(ClOrdId)}: {ClOrdId}, {nameof(AccountId)}: {AccountId}, {nameof(ContractId)}: {ContractId}, {nameof(OrderId)}: {OrderId}, {nameof(ExecId)}: {ExecId}, {nameof(StrategyPositionId)}: {StrategyPositionId}, {nameof(SignalGroupId)}: {SignalGroupId}, {nameof(PositionEffect)}: {PositionEffect}, {nameof(Side)}: {Side}, {nameof(Volume)}: {Volume}, {nameof(Price)}: {Price}, {nameof(Commission)}: {Commission}, {nameof(Dt)}: {Dt}, {nameof(ExecutionRequestId)}: {ExecutionRequestId}, {nameof(ExternalTradeId)}: {ExternalTradeId}, {nameof(Commissions)}: {Commissions}, {nameof(PaymentCurrencyId)}: {PaymentCurrencyId}, {nameof(FxRate)}: {FxRate}, {nameof(CalculatedCcyLastQty)}: {CalculatedCcyLastQty}, {nameof(ParentPositionId)}: {ParentPositionId}, {nameof(IsSynthetic)}: {IsSynthetic} }}";
        }
    }
}