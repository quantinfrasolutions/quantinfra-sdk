using System;
using System.Collections.Generic;
using System.Linq;
using Common.Trading.Positions;
using Microsoft.Extensions.Logging;
using NodaTime;
using QuantInfra.Sdk.MarketData;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Strategies
{
    public abstract class AbstractSingleBarHostedStrategy<TParams> : AbstractHostedStrategy<TParams>
        where TParams : class, new()
    {
        public const string MainBarStorageName = "main";
        
        protected int SkipBars { get; private set; }
        Dictionary<string, AbstractIndicator> _indicators = new Dictionary<string, AbstractIndicator>();

        public AbstractSingleBarHostedStrategy(StrategyConfig sc) : base(sc)
        {
            
        }

        protected IBarStorage BarStorage { get; private set; }
        protected IBar CurrentBar => BarStorage.CurrentBar;


        protected override void OnInitialize(StrategyInitializationContext initContext)
        {
            BarStorage = ClaimBarStorage(StrategyConfig.RequiredBarStorages[MainBarStorageName]);
            foreach (var s in StrategyConfig.Symbols)
            {
                ClaimContract(s.Value, s.Key);
            }
            OnInitialized(initContext);
        }
        
        /// <summary>
        /// Claim indicators here
        /// </summary>
        protected virtual void OnInitialized(StrategyInitializationContext initContext) { }
        

        protected override bool OnBarClosed(string barQualifier, StrategyCalculationContext context)
        {
            if (BarStorage.Count < SkipBars) return false;
            CalculateVector(barQualifier, context);
            return true;
        }

        protected abstract void CalculateVector(string barQualifier, StrategyCalculationContext context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indicator"></param>
        /// <param name="requiredBars">Number of bars used by a strategy. E.g. if a strategy requires a value of an indicator from 2 bars ago (Bars[2]), RequiredBars must be set to 3</param>
        protected void RegisterIndicator(AbstractIndicator indicator, int lookback = 0, string? logName = null)
        {
            SkipBars = Math.Max(SkipBars, BarStorage.RegisterIndicator(indicator, lookback));
            if (!string.IsNullOrEmpty(logName))
            {
                _indicators.Add(logName, indicator);
            }
        }

        protected void NewOrder(
            decimal signedVolume,
            PositionEffect positionEffect,
            string clOrdId = null,
            OrdType ordType = OrdType.Market,
            double? price = null,
            double? stopPx = null,
            bool isSuspended = false,
            string? positionId = null,
            IReadOnlyDictionary<string, LinkType> linkedOrders = null,
            bool isSLTP = false,
            PegInstructions? pegInstructions = null
        ) => NewOrder(MainSymbolName, signedVolume, positionEffect, clOrdId, ordType, price,
            stopPx, isSuspended, positionId, linkedOrders, isSLTP, pegInstructions, tradingSessionIds: BarStorage.BarStorageConfig.TradingSessionIds); // HACK, TODO: trading session ids are assigned only for single bar strategies

        protected void OpenPosition(
            decimal signedVolume,
            double? stopLoss = null,
            double? takeProfit = null,
            string? positionId = null,
            decimal? trailingStopStep = null,
            OrdType ordType = OrdType.Market,
            double? price = null,
            int? orderValiditySeconds = null
        ) =>
            OpenPosition(MainSymbolName, signedVolume, stopLoss, takeProfit, positionId, trailingStopStep, ordType, price, orderValiditySeconds: orderValiditySeconds, tradingSessionIds: BarStorage.BarStorageConfig.TradingSessionIds);        

        // TODO: trading session ids
        protected void ClosePosition(decimal newSignedVolume = 0, string? positionId = null, int? orderValiditySeconds = null) =>
            ClosePosition(MainSymbolName, newSignedVolume, positionId, orderValiditySeconds: orderValiditySeconds);

        protected void ChangePositionSL(double price, double? trailingStopLossStep = null, string? positionId = null) =>
            ChangePositionSl(MainSymbolName, price, positionId);

        protected void CancelPositionSL(string? positionId = null) =>
            CancelPositionSl(MainSymbolName, positionId);

        protected void ChangePositionTP(double price, string? positionId = null) =>
            ChangePositionTp(MainSymbolName, price, positionId);

        protected void CancelPositionTP(string? positionId = null) =>
            CancelPositionTp(MainSymbolName, positionId);

        protected OrderStatus? GetPositionStopLoss(string? positionId = null) =>
            GetPositionStopLoss(StrategyConfig.Symbols[MainSymbolName], positionId);

        protected OrderStatus? GetPositionTakeProfit(string? positionId = null) =>
            GetPositionTakeProfit(StrategyConfig.Symbols[MainSymbolName], positionId);

        protected bool IsInPosition() => throw new NotSupportedException();// IsInPosition(MainSymbolName);

        protected bool IsActiveOpenOrdersLessThan(int orderCountLimit = 1) =>
            throw new NotSupportedException();
            // GetOrdersByContract(MainSymbolName).Count(o => !o.IsSltp && o.ContractId == StrategyConfig.Symbols[MainSymbolName]) < orderCountLimit;

            /// <summary>
            /// Do not use for multi-share strategies or if you assign PositionId manually.
            /// The function returns a position with the default position ID, so you may get false results.
            /// </summary>
        protected Position? GetCurrentPosition() => throw new NotSupportedException();
            // GetPosition();

        protected Position? GetPosition(string positionId) =>
            GetPositionsByContract().SingleOrDefault(p => p.StrategyPositionId == positionId);
        
        protected decimal GetVolume(decimal value, string symbol = MainSymbolName) 
            => GetVolume(GetContractId(symbol), value); 

        public int GetBarsCountSinceMoment(Instant startDate, int timeframe) => throw new NotImplementedException();// (int)((BarStorage.CurrentBar.OpenDt - BarStorage.GetBarOpeningDt(startDate)).TotalMinutes) / timeframe + (BarStorage.CurrentBar.Closed ? 1 : 0);
        public int GetBarsCountSinceMoment(Instant startDate) => throw new NotImplementedException(); // GetBarsCountSinceMoment(startDate, StrategyConfig.MainTimeframe);


        private void LogIndicators()
        {
            Logger.LogTrace($"{{{CurrentBar.BarToLogFormat},'indicators':{{{string.Join(',', _indicators.Select(i => $"'{i.Key}':{i.Value.GetLogInformation(CurrentBar)}"))}}}}}");
        }
    }
}
