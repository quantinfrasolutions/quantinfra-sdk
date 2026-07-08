using System;
using System.Collections.Generic;
using System.Diagnostics;
using NodaTime;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Trading.Positions
{
    public class Position
    {
        public long OpenTradeId { get; init; }
        public int AccountId { get; init; }
        public string? StrategyPositionId { get; init; }
        public int ContractId { get; init; }        
        public decimal Volume { get; init; }
        public Side Side { get; init; }
        public decimal OpenPrice { get; init; }
        public decimal TotalOpenPayments { get; init; }
        public Instant OpenDt { get; init; }
        public Instant HistoryOpenDt { get; init; }
        public decimal TotalSettlPayments { get; init; }
        public decimal TotalSettlPaymentsInAccountCcy { get; init; }
        public decimal SettlPrice { get; init; }
        public long? CloseTradeId { get; init; }
        public decimal? ClosePrice { get; init; }
        public Instant? CloseDt { get; init; }
        public bool IsClosed => CloseDt.HasValue;
        /// <summary>
        /// Gross PnL accrued in the result of closing this part of the position
        /// </summary>
        public decimal RealizedPnL { get; init; }
        public decimal RealizedPnLInAccountCcy { get; init; }
        /// <summary>
        /// Daily floating PnL
        /// </summary>
        public decimal FloatingPnL { get; init; }
        /// <summary>
        /// Floating PnL accrued since the position opening
        /// </summary>
        public decimal TotalFloatingPnL { get; init; }
        /// <summary>
        /// Commission from open and close trades attributed to the closed part of the position
        /// </summary>
        public decimal Commission { get; init; }
        public long? SignalGroupId { get; init; }
        public bool IsSynthetic { get; init; }
        /// <summary>
        /// For positions in underlying contracts, contains the reference to the position in the synthetic contract.
        /// For other positions equals PositionId.
        /// </summary>
        public long ParentPositionId { get; init; }

        public decimal SignedVolume
        {
            get => Volume * Side.GetSign();
            init
            {
                Volume = Math.Abs(value);
                Side = value.FromSign();
            }
        }

        public Position() { }

        public Position(Position p)
        {
            OpenTradeId = p.OpenTradeId;
            AccountId = p.AccountId;
            StrategyPositionId = p.StrategyPositionId;
            ContractId = p.ContractId;
            Volume = p.Volume;
            TotalOpenPayments = p.TotalOpenPayments;
            TotalSettlPayments = p.TotalSettlPayments;
            TotalSettlPaymentsInAccountCcy = p.TotalSettlPaymentsInAccountCcy;
            Side = p.Side;
            OpenPrice = p.OpenPrice;
            SettlPrice = p.SettlPrice;
            OpenDt = p.OpenDt;
            HistoryOpenDt = p.HistoryOpenDt;
            CloseTradeId = p.CloseTradeId;
            ClosePrice = p.ClosePrice;
            CloseDt = p.CloseDt;
            RealizedPnL = p.RealizedPnL;
            RealizedPnLInAccountCcy = p.RealizedPnLInAccountCcy;
            FloatingPnL = p.FloatingPnL;
            TotalFloatingPnL = p.TotalFloatingPnL;
            Commission = p.Commission;
            SignalGroupId = p.SignalGroupId;
            IsSynthetic = p.IsSynthetic;
            ParentPositionId = p.ParentPositionId;
        }

        public static Position OpenPosition(
            long tradeId,
            int contractId,
            int accountId,
            decimal volume,
            Side side,
            decimal openPrice,
            decimal calculatedCcyLastQty,
            decimal fxRate,
            int accountCcyPrecision,
            Instant openDt,
            bool isSynthetic,
            long? parentPositionId,
            string? strategyPositionId = null,
            decimal commission = 0m,
            long? signalGroupId = null
        )
        {
            if (volume <= 0) throw new ArgumentException($"Cannot open position with negative volume");
            if (calculatedCcyLastQty <= 0) Debugger.Break();
            return new Position
            {
                OpenTradeId = tradeId,
                ContractId = contractId,
                AccountId = accountId,
                Volume = volume,
                Side = side,
                OpenPrice = openPrice,
                SettlPrice = openPrice,
                OpenDt = openDt,
                HistoryOpenDt = openDt,
                TotalOpenPayments = calculatedCcyLastQty,
                TotalSettlPayments = calculatedCcyLastQty,
                TotalSettlPaymentsInAccountCcy = Math.Round(calculatedCcyLastQty * fxRate, accountCcyPrecision),
                ParentPositionId = parentPositionId ?? tradeId,
                StrategyPositionId = strategyPositionId,
                Commission = commission,
                SignalGroupId = signalGroupId,
                IsSynthetic = isSynthetic
            };
        }

        public static Position OpenPosition(
            long tradeId,
            int contractId,
            int accountId,
            decimal signedVolume,
            decimal openPrice,
            decimal calculatedCcyLastQty,
            decimal fxRate,
            int accountCcyPrecision,
            Instant openDt,
            bool isSynthetic,
            long? parentPositionId,
            string? strategyPositionId = null,
            decimal commission = 0m,
            long? signalGroupId = null
        ) => OpenPosition(
            tradeId,
            contractId,
            accountId,
            Math.Abs(signedVolume),
            signedVolume.FromSign(),
            openPrice,
            calculatedCcyLastQty,
            fxRate,
            accountCcyPrecision,
            openDt,
            isSynthetic,
            parentPositionId,
            strategyPositionId,
            commission,
            signalGroupId
        );

        public (Position actualPosition, Position historyRecord) IncreasePosition(
            long tradeId,
            decimal volume,
            decimal calculatedCcyLastQty,
            decimal fxRate,
            int accountCcyPrecision,
            Instant dt,
            decimal commission,
            long? parentPositionId,
            IPnLCalculator calculator
        )
        {
            if (volume < 0) throw new ArgumentException($"Cannot increase position by negative volume");
            
            var newOpenPayments = TotalOpenPayments + calculatedCcyLastQty;
            var newSettlPayments = TotalSettlPayments + calculatedCcyLastQty;
            var newOpenPrice = calculator.GetPrice(Volume, newOpenPayments);
            var newSettlPrice = calculator.GetPrice(Volume, newSettlPayments);
            
            return (
                new Position(this)
                {
                    OpenTradeId = tradeId,
                    TotalOpenPayments = newOpenPayments,
                    OpenPrice = newOpenPrice,
                    TotalSettlPayments = newSettlPayments,
                    SettlPrice = newSettlPrice,
                    TotalSettlPaymentsInAccountCcy = TotalSettlPaymentsInAccountCcy + Math.Round(calculatedCcyLastQty * fxRate, accountCcyPrecision),
                    Volume = Volume + volume,
                    HistoryOpenDt = dt,
                    Commission = Commission + commission,
                    ParentPositionId = parentPositionId ?? tradeId,
                },
                new Position(this)
                {
                    CloseDt = dt,
                    CloseTradeId = tradeId,
                }
            );
        }

        public (Position? actualPosition, Position historyRecord) ReducePosition(
            long tradeId,
            decimal volume,
            decimal price,
            decimal calculatedCcyLastQty,
            decimal fxRate,
            int accountCcyPrecision,
            Instant dt,
            decimal commission,
            int settlCcyPrecision,
            long? parentPositionId,
            IPnLCalculator calculator
        )
        {
            if (volume < 0) throw new ArgumentException($"Cannot reduce position by negative volume");
            var closeVolume = Math.Min(Volume, volume);            

            if (closeVolume > Volume)
            {
                throw new ArgumentException($"Cannot close more ({volume}) than the position volume ({Volume})");
            }
            
            var newVolume = Volume - closeVolume;
            decimal newTotalPayment = 0, newTotalSettlPayment = 0, newTotalSettlPaymentInAccountCcy = 0, newTotalFloatingPnl = 0;
            if (newVolume != 0)
            {
                newTotalPayment = Math.Round(TotalOpenPayments * newVolume / Volume, settlCcyPrecision);
                newTotalSettlPayment = Math.Round(TotalSettlPayments * newVolume / Volume, settlCcyPrecision);
                newTotalSettlPaymentInAccountCcy = Math.Round(TotalSettlPaymentsInAccountCcy * newVolume / Volume, settlCcyPrecision);
                newTotalFloatingPnl = Math.Round(TotalFloatingPnL * newVolume / Volume, settlCcyPrecision);
            }

            var realizedPnL = calculator.GetPnL((newTotalPayment - TotalOpenPayments) * Side.Invert().GetSign(), calculatedCcyLastQty * Side.GetSign());
            var realizedPnLInAccountCcy = Math.Round(realizedPnL * fxRate, accountCcyPrecision);
            
            var commissionBroughtForward = Math.Round(newVolume / Volume * Commission, settlCcyPrecision);
            var closedPartCommission = Commission - commissionBroughtForward + commission;
                 

            return (
                newVolume == 0 ? null :
                    new Position(this)
                    {
                        Volume = newVolume,
                        TotalOpenPayments = newTotalPayment,
                        TotalSettlPayments = newTotalSettlPayment,
                        TotalSettlPaymentsInAccountCcy = newTotalSettlPaymentInAccountCcy,
                        Commission = commissionBroughtForward,
                        ParentPositionId = parentPositionId ?? tradeId,
                        TotalFloatingPnL = newTotalFloatingPnl,
                    },
                new Position(this)
                {
                    Volume = closeVolume,
                    RealizedPnL = realizedPnL,
                    RealizedPnLInAccountCcy = realizedPnLInAccountCcy,
                    CloseDt = dt,
                    ClosePrice = price,
                    Commission = closedPartCommission,
                    CloseTradeId = tradeId,
                    TotalOpenPayments = TotalOpenPayments - newTotalPayment,
                    TotalSettlPayments = TotalSettlPayments - newTotalSettlPayment,
                    TotalSettlPaymentsInAccountCcy = TotalSettlPaymentsInAccountCcy - newTotalSettlPaymentInAccountCcy,
                    FloatingPnL = -(TotalFloatingPnL - newTotalFloatingPnl), // When position closes, the total accumulated floating PnL must be removed to give the zero sum over all position history records
                    TotalFloatingPnL = 0,
                }
            );
        }

        public (Position? actualPosition, Position historyRecord) ChangePosition(
            long tradeId,
            decimal signedVolume,
            decimal price,
            decimal calculatedCcyLastQty,
            decimal fxRate,
            int accountCcyPrecision,
            Instant dt,
            decimal commission,
            int settlCcyPrecision,
            long? parentPositionId,
            IPnLCalculator calculator
        )
        {
            if (Side == signedVolume.FromSign())
            {
                return IncreasePosition(tradeId, Math.Abs(signedVolume), calculatedCcyLastQty, fxRate, accountCcyPrecision, dt, commission, parentPositionId, calculator);
            }

            var absVolume = Math.Abs(signedVolume);

            if (absVolume <= Volume)
            {
                return ReducePosition(tradeId, absVolume, price, calculatedCcyLastQty, fxRate, accountCcyPrecision, dt, commission, settlCcyPrecision, parentPositionId, calculator);
            }

            var reopenVolume = absVolume - Volume;
            
            var commissionBroughtForward = Math.Round(reopenVolume / absVolume * commission, settlCcyPrecision);
            var closingCommission = commission - commissionBroughtForward;
            
            var paymentBroughtForward = Math.Round(reopenVolume / absVolume * calculatedCcyLastQty, settlCcyPrecision);
            var closingPayment = calculatedCcyLastQty - paymentBroughtForward;
            
            var (_, historyRecord) = ReducePosition(tradeId, Volume, price, closingPayment, fxRate,
                accountCcyPrecision, dt, closingCommission, settlCcyPrecision, parentPositionId, calculator);
            var actualPosition = OpenPosition(tradeId, ContractId, AccountId, reopenVolume, signedVolume.FromSign(), 
                price, paymentBroughtForward, fxRate, accountCcyPrecision, dt, IsSynthetic, parentPositionId,
                strategyPositionId: StrategyPositionId, commission: commissionBroughtForward, signalGroupId: SignalGroupId);

            return (actualPosition, historyRecord);
        }

        public (Position actualPosition, Position historyRecord) MarkToMarket(
            Instant dt,
            decimal settlPrice,
            decimal signedValue,
            decimal signedValueInAccountCcy,
            IPnLCalculator calculator
        )
        {
            var floatingPnL = calculator.GetPnL(TotalSettlPayments * Side.GetSign(), signedValue);
            var totalFloatingPnL = TotalFloatingPnL + floatingPnL;
            
            // TotalSettlPayments/TotalOpenPayments are positive for both long and short positions
            // However, when marking to market, it may happen that the position value is negative (if the price is negative)
            // To accommodate this, the signed value is passed to this function, and it needs to be multiplied by the side
            return (
                new Position(this)
                {
                    HistoryOpenDt = dt,
                    TotalSettlPayments = signedValue * Side.GetSign(),
                    SettlPrice = settlPrice,
                    TotalSettlPaymentsInAccountCcy = signedValueInAccountCcy * Side.GetSign(),
                    TotalFloatingPnL = totalFloatingPnL,
                },
                new Position(this)
                {
                    CloseDt = dt,
                    ClosePrice = settlPrice,
                    FloatingPnL = floatingPnL,
                    TotalFloatingPnL = totalFloatingPnL,
                }
            );
        }

        public static (Position? actualPosition, IReadOnlyList<Position> historyRecords) AggregatePositions(IReadOnlyCollection<Position> positions, Instant dt, IPnLCalculator calculator,
            decimal returnCalculationBase)
        {
            throw new NotImplementedException();
            // if (positions.Count == 0) return (null, new List<Position>());
            // if (positions.Select(p => p.ContractId).Distinct().Count() > 1) throw new InvalidOperationException("Cannot aggregate positions with different contract ids");
            //
            // var historyRecords = new List<Position>(positions.Count + 1);
            //
            // var resultingSide = positions.Sum(p => p.SignedVolume) >= 0 ? Side.Buy : Side.Sell;
            //
            // Position? currentActualPosition = null;
            // foreach (var pos in positions.Where(p => p.Side == resultingSide).OrderBy(p => p.OpenDt))
            // {
            //     if (currentActualPosition == null)
            //     {
            //         currentActualPosition = Position.OpenPosition(null, pos.ContractId, pos.AccountId, pos.SignedVolume, pos.SettlPrice, dt, pos.IsSynthetic, pos.ParentPositionId);
            //     }
            //     else
            //     {
            //         // No PnL is generated here
            //         (currentActualPosition, _) = currentActualPosition.IncreasePosition(pos.Volume, pos.SettlPrice, dt, calculator, 0);
            //     }
            //     
            //     historyRecords.Add(pos.ReducePosition(pos.Volume, pos.SettlPrice, dt, calculator, returnCalculationBase).historyRecord);
            // }
            //
            // var settlPrice = currentActualPosition!.SettlPrice;
            //
            // foreach (var pos in positions.Where(p => p.Side != resultingSide).OrderBy(p => p.OpenDt))
            // {
            //     (currentActualPosition, _) = currentActualPosition!.ReducePosition(pos.Volume,
            //         currentActualPosition.SettlPrice, dt, calculator, returnCalculationBase);
            //     
            //     // PnL is attributed to the closed positions with the opposite side
            //     historyRecords.Add(pos.ReducePosition(pos.Volume, settlPrice, dt, calculator, returnCalculationBase).historyRecord);
            // }
            //
            // return (currentActualPosition, historyRecords);
        }

        protected bool Equals(Position other)
        {
            return OpenTradeId == other.OpenTradeId &&
                   AccountId == other.AccountId &&
                   StrategyPositionId == other.StrategyPositionId &&
                   ContractId == other.ContractId &&
                   Volume == other.Volume &&
                   Side == other.Side &&
                   OpenPrice == other.OpenPrice &&
                   TotalOpenPayments == other.TotalOpenPayments &&
                   OpenDt.Equals(other.OpenDt) &&
                   HistoryOpenDt.Equals(other.HistoryOpenDt) &&
                   SettlPrice == other.SettlPrice &&
                   TotalSettlPayments == other.TotalSettlPayments &&
                   TotalSettlPaymentsInAccountCcy == other.TotalSettlPaymentsInAccountCcy &&
                   CloseTradeId == other.CloseTradeId &&
                   ClosePrice == other.ClosePrice &&
                   Nullable.Equals(CloseDt,
                       other.CloseDt) &&
                   RealizedPnL == other.RealizedPnL &&
                   RealizedPnLInAccountCcy == other.RealizedPnLInAccountCcy &&
                   FloatingPnL == other.FloatingPnL &&
                   TotalFloatingPnL == other.TotalFloatingPnL &&
                   Commission == other.Commission &&
                   Nullable.Equals(SignalGroupId,
                       other.SignalGroupId) &&
                   IsSynthetic == other.IsSynthetic &&
                   ParentPositionId == other.ParentPositionId;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Position)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(OpenTradeId);
            hashCode.Add(AccountId);
            hashCode.Add(StrategyPositionId);
            hashCode.Add(ContractId);
            hashCode.Add(Volume);
            hashCode.Add((int)Side);
            hashCode.Add(TotalOpenPayments);
            hashCode.Add(OpenDt);
            hashCode.Add(HistoryOpenDt);
            hashCode.Add(TotalSettlPayments);
            hashCode.Add(TotalSettlPaymentsInAccountCcy);
            hashCode.Add(CloseTradeId);
            hashCode.Add(ClosePrice);
            hashCode.Add(CloseDt);
            hashCode.Add(RealizedPnL);
            hashCode.Add(RealizedPnLInAccountCcy);
            hashCode.Add(FloatingPnL);
            hashCode.Add(TotalFloatingPnL);
            hashCode.Add(Commission);
            hashCode.Add(SignalGroupId);
            hashCode.Add(IsSynthetic);
            hashCode.Add(ParentPositionId);
            return hashCode.ToHashCode();
        }

        // public override string ToString()
        // {
        //     return
        //         $"{nameof(OpenTradeId)}: {OpenTradeId}, {nameof(AccountId)}: {AccountId}, {nameof(StrategyPositionId)}: {StrategyPositionId}, {nameof(ContractId)}: {ContractId}, {nameof(Volume)}: {Volume}, {nameof(Side)}: {Side}, {nameof(OpenPrice)}: {OpenPrice}, {nameof(TotalOpenPayments)}: {TotalOpenPayments}, {nameof(OpenDt)}: {OpenDt}, {nameof(HistoryOpenDt)}: {HistoryOpenDt}, {nameof(TotalSettlPayments)}: {TotalSettlPayments}, {nameof(TotalSettlPaymentsInAccountCcy)}: {TotalSettlPaymentsInAccountCcy}, {nameof(SettlPrice)}: {SettlPrice}, {nameof(CloseTradeId)}: {CloseTradeId}, {nameof(ClosePrice)}: {ClosePrice}, {nameof(CloseDt)}: {CloseDt}, {nameof(IsClosed)}: {IsClosed}, {nameof(RealizedPnL)}: {RealizedPnL}, {nameof(RealizedPnLInAccountCcy)}: {RealizedPnLInAccountCcy}, {nameof(FloatingPnL)}: {FloatingPnL}, {nameof(TotalFloatingPnL)}: {TotalFloatingPnL}, {nameof(Commission)}: {Commission}, {nameof(SignalGroupId)}: {SignalGroupId}, {nameof(IsSynthetic)}: {IsSynthetic}, {nameof(ParentPositionId)}: {ParentPositionId}, {nameof(SignedVolume)}: {SignedVolume}";
        // }

        public override string ToString()
        {
            return
                $"{nameof(Volume)}: {Volume}, {nameof(Side)}: {Side}, {nameof(OpenPrice)}: {OpenPrice}, {nameof(OpenDt)}: {OpenDt}, {nameof(ClosePrice)}: {ClosePrice}, {nameof(CloseDt)}: {CloseDt}";
        }

        /// <summary>
        /// Calculates the trades required to be applied for the current position to become equal to the target position
        /// </summary>
        public List<Trade> GetOffsetTrades(string accountServiceName, ITradeIdProvider tradeIdProvider, int accountId, int contractId, 
            decimal targetSignedVolume, decimal targetPayments, Instant dt, IPnLCalculator calculator, int paymentCurrencyId, 
            decimal fxRate
        )
        {
            var res = new List<Trade>(2);

            long? openTradeId = targetSignedVolume != 0 ? tradeIdProvider.GetNextTradeId() : null;
            if (Volume != 0)
            {
                res.Add(new Trade(
                    accountServiceName, 
                    tradeIdProvider.GetNextTradeId(), 
                    $"Offset {dt}-C", 
                    accountId, contractId, null, null, StrategyPositionId, null, 
                    PositionEffect.Close, Side.Invert(), Volume, SettlPrice, 
                    0, dt, null, null, 
                    new Dictionary<int, decimal>(),
                    paymentCurrencyId, fxRate,
                    TotalSettlPayments,
                    ParentPositionId, openTradeId, IsSynthetic
                ));
            }
            if (targetSignedVolume != 0)
            {
                openTradeId = tradeIdProvider.GetNextTradeId();
                // Reopen the position by the given price
                res.Add(new Trade(
                    accountServiceName, 
                    openTradeId!.Value, 
                    $"Offset {dt}-O", 
                    accountId, contractId, null, null, StrategyPositionId, null, 
                    PositionEffect.Open, targetSignedVolume.FromSign(), Math.Abs(targetSignedVolume), calculator.GetPrice(targetSignedVolume, targetPayments * Math.Sign(targetSignedVolume)), 
                    0, dt, null, null,
                    new Dictionary<int, decimal>(),
                    paymentCurrencyId, fxRate,
                    targetPayments, 
                    ParentPositionId, null, IsSynthetic
                ));
            }

            // var targetOpenPrice = calculator.GetPrice(Math.Abs(targetSignedVolume), targetPayments);
            //
            // if ((targetSignedVolume == SignedVolume && targetPayments != TotalOpenPayments)    // Volumes are equal, prices are not
            //     || (targetSignedVolume.FromSign() == Side && Math.Abs(targetSignedVolume) < Volume && targetPayments != TotalOpenPayments) // The position has been decreased and the price is different
            //     || targetSignedVolume == 0 
            // )
            // {
            //     long? openTradeId = targetSignedVolume != 0 ? tradeIdProvider.GetNextTradeId() : null;
            //     // Close current position by its current SettlPrice
            //     res.Add(new Trade(
            //         accountServiceName, 
            //         tradeIdProvider.GetNextTradeId(), 
            //         $"Offset {dt}-C", 
            //         accountId, contractId, null, null, StrategyPositionId, null, 
            //         PositionEffect.Close, Side.Invert(), Volume, SettlPrice, 
            //         0, dt, null, null, 
            //         new Dictionary<int, decimal>(),
            //         paymentCurrencyId, fxRate,
            //         TotalSettlPayments, 
            //         ParentPositionId, openTradeId, IsSynthetic
            //     ));
            //     if (targetSignedVolume != 0)
            //     {
            //         // Reopen the position by the given price
            //         res.Add(new Trade(
            //             accountServiceName, 
            //             openTradeId!.Value, 
            //             $"Offset {dt}-O", 
            //             accountId, contractId, null, null, StrategyPositionId, null, 
            //             PositionEffect.Open, Side, Math.Abs(targetSignedVolume), targetOpenPrice, 
            //             0, dt, null, null,
            //             new Dictionary<int, decimal>(),
            //             paymentCurrencyId, fxRate,
            //             targetPayments, 
            //             ParentPositionId, null, IsSynthetic
            //         ));
            //     }
            // }
            // else if (targetSignedVolume != SignedVolume)
            // {
            //     var volumeDiff = Math.Abs(targetSignedVolume - SignedVolume);
            //     if (Volume == 0)
            //     {
            //         res.Add(new Trade(
            //             accountServiceName,
            //             tradeIdProvider.GetNextTradeId(),
            //             $"Offset {dt}", 
            //             accountId, contractId, null, null, StrategyPositionId, null, 
            //             PositionEffect.Open, targetSignedVolume.FromSign(), volumeDiff, targetOpenPrice, 
            //             0, dt, null, null, 
            //             new Dictionary<int, decimal>(),
            //             paymentCurrencyId, fxRate,
            //             targetPayments, 
            //             ParentPositionId, null, IsSynthetic
            //         ));
            //     }
            //     else if (Math.Sign(targetSignedVolume) == Math.Sign(SignedVolume))
            //     {
            //         // Position is increased
            //         if (Math.Abs(targetSignedVolume) > Volume)
            //         {
            //             var price = calculator.GetOffsetTradePrice(Volume, volumeDiff, OpenPrice, targetOpenPrice, precision);
            //             res.Add(new Trade(
            //                 accountServiceName, 
            //                 tradeIdProvider.GetNextTradeId(), 
            //                 $"Offset {dt}", 
            //                 accountId, contractId, null, null, StrategyPositionId, null, 
            //                 PositionEffect.Open, Side, volumeDiff, price, 
            //                 0, dt, null, null, 
            //                 new Dictionary<int, decimal>(),
            //                 paymentCurrencyId, fxRate,
            //                 calculator.GetValueInSettlementCcy(price, volumeDiff), 
            //                 ParentPositionId, null, IsSynthetic
            //             ));
            //         }
            //         // Position is decreased
            //         else
            //         {
            //             // It is not possible to calculate the close price, so just use the current SettlPrice
            //             res.Add(new Trade(
            //                 accountServiceName,
            //                 tradeIdProvider.GetNextTradeId(), 
            //                 $"Offset {dt}", 
            //                 accountId, contractId, null, null, StrategyPositionId, null, 
            //                 PositionEffect.Close, Side.Invert(), volumeDiff, SettlPrice, 
            //                 0, dt, null, null, 
            //                 new Dictionary<int, decimal>(),
            //                 paymentCurrencyId, fxRate,
            //                 calculator.GetValueInSettlementCcy(SettlPrice, volumeDiff), 
            //                 ParentPositionId, null, IsSynthetic
            //             ));
            //         }
            //     }
            //     // Position turned over
            //     else
            //     {
            //         res.Add(new Trade(
            //             accountServiceName,
            //             tradeIdProvider.GetNextTradeId(), 
            //             $"Offset {dt}", accountId, contractId, null, null, StrategyPositionId, null, 
            //             PositionEffect.Open, Side.Invert(), volumeDiff, targetOpenPrice, 
            //             0, dt, null, null, 
            //             new Dictionary<int, decimal>(),
            //             paymentCurrencyId, fxRate,
            //             calculator.GetValueInSettlementCcy(targetOpenPrice, volumeDiff), 
            //             ParentPositionId, null, IsSynthetic
            //         ));
            //     }
            // }
            // // else: targetSignedVolume == SignedVolume && targetOpenPrice == OpenPrice: do nothing
            
            return res;
        }
    }
}
