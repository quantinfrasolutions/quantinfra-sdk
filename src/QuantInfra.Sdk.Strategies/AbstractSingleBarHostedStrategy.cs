using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NodaTime;
using QuantInfra.Sdk.MarketData;
using QuantInfra.Sdk.Trading.Orders;
using QuantInfra.Sdk.Trading.Positions;

namespace QuantInfra.Sdk.Strategies
{
    public abstract class AbstractSingleBarHostedStrategy<TParams> : AbstractHostedStrategy<TParams>
        where TParams : class, new()
    {
        private int _skipBars;
        private readonly Dictionary<string, AbstractIndicator> _indicators = new();
        
        protected int ContractId { get; }
        protected string BarStorageName { get; }

        public AbstractSingleBarHostedStrategy(Strategy sc) : base(sc)
        {
            ContractId = sc.Symbols.Values.Single();
            BarStorageName = sc.RequiredBarStorages.Keys.Single();
        }

        protected IBarStorage BarStorage { get; private set; }
        protected IBar CurrentBar => BarStorage.CurrentBar;


        protected override void OnInitialize(StrategyInitializationContext initContext)
        {
            BarStorage = ClaimBarStorage(StrategyConfig.RequiredBarStorages[BarStorageName]);
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
        

        protected override void OnBarClosed(string barQualifier, StrategyCalculationContext context)
        {
            if (BarStorage.Count < _skipBars) return;
            CalculateVector(barQualifier, context);
        }

        protected abstract void CalculateVector(string barQualifier, StrategyCalculationContext context);
        
        /// <param name="indicator"></param>
        /// <param name="lookback">How many historical values are required</param>
        protected void RegisterIndicator(AbstractIndicator indicator, int lookback = 0, string? logName = null)
        {
            _skipBars = Math.Max(_skipBars, BarStorage.RegisterIndicator(indicator, lookback));
            if (!string.IsNullOrEmpty(logName))
            {
                _indicators.Add(logName, indicator);
            }
        }
        
        /// <summary>
        /// Opens new position
        /// </summary>
        public void OpenPosition(
            decimal signedVolume,
            double? stopLoss = null,
            double? takeProfit = null,
            string? positionId = null,
            OrdType ordType = OrdType.Market,
            double? price = null,
            double? stopPx = null,
            int? orderValiditySeconds = null
        ) => OpenPosition(ContractId, signedVolume, stopLoss, takeProfit, positionId,
            ordType, price, stopPx, orderValiditySeconds);
        
        public void ClosePosition(string? positionId = null, string? clOrdId = null) => 
            ClosePosition(ContractId, positionId, clOrdId);
        
        protected void ChangePositionSL(double price, double? trailingStopLossStep = null, string? positionId = null) =>
            ChangePositionSl(ContractId, price, positionId);

        protected void CancelPositionSL(string? positionId = null) =>
            CancelPositionSl(ContractId, positionId);

        protected void ChangePositionTP(double price, string? positionId = null) =>
            ChangePositionTp(ContractId, price, positionId);

        protected void CancelPositionTP(string? positionId = null) =>
            CancelPositionTp(ContractId, positionId);

        protected OrderStatus? GetPositionStopLoss(string? positionId = null) =>
            GetPositionStopLoss(ContractId, positionId);

        protected OrderStatus? GetPositionTakeProfit(string? positionId = null) =>
            GetPositionTakeProfit(ContractId, positionId);
        
        protected decimal GetVolume(decimal? value = null, decimal? price = null, bool normalize = true) 
            => GetVolume(ContractId, value, price, normalize); 
        
        public string GetOpenPositionClOrdId(string? positionId = null) => $"{positionId}-o";
        public string GetClosePositionClOrdId(string? positionId = null) => $"{positionId}-c";
        public string GetStopLossClOrdId(string? positionId = null) => $"{positionId}-sl";
        public string GetTakeProfitClOrdId(string? positionId = null) => $"{positionId}-tp";

        protected void LogIndicators(LogLevel logLevel = LogLevel.Trace)
        {
            Logger.Log(logLevel,
                "{{ {bar}, 'indicators': {{ {indicators} }} }}",
                CurrentBar.BarToLogFormat,
                string.Join(',', _indicators.Select(i => 
                    $"'{i.Key}': {i.Value.GetLogInformation(CurrentBar)}"
                ))
            );
        }
    }
}
