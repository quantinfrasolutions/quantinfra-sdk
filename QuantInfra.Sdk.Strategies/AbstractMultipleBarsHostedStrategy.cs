using System;
using System.Collections.Generic;
using System.Linq;
using QuantInfra.Sdk.MarketData;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Strategies;

public abstract class AbstractMultipleBarsHostedStrategy<TParams> : AbstractHostedStrategy<TParams>
    where TParams : class, new()
{
    public const string MainBarStorageName = "main";

    private readonly Dictionary<string, int> _skipBars = new();
    private readonly Dictionary<string, string> _bsCommonToLocal = new();

    public AbstractMultipleBarsHostedStrategy(StrategyConfig sc) : base(sc)
    {
            
    }

    private Dictionary<string, IBarStorage> _allBarStorages = new();
    private Dictionary<string, IBarStorage> _currentCycleBarStorages = new();
    protected IReadOnlyDictionary<string, IBarStorage> BarStorages => _currentCycleBarStorages;


    protected override void OnInitialize(StrategyInitializationContext initContext)
    {
        foreach (var bs in StrategyConfig.RequiredBarStorages)
        {
            _allBarStorages.Add(bs.Key, ClaimBarStorage(bs.Value));
            _bsCommonToLocal.Add(bs.Value.FullQualifier, bs.Key);
            _skipBars.Add(bs.Key, 0);
        }
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
        var localName = _bsCommonToLocal[barQualifier];
        _currentCycleBarStorages[localName] = _allBarStorages[localName];
            
        if (_currentCycleBarStorages.Count == _allBarStorages.Count)
        {
            if (_currentCycleBarStorages.Keys.Any(bs => _currentCycleBarStorages[bs].Count < _skipBars[bs])) return false;
            CalculateVector(barQualifier, context);
            _currentCycleBarStorages.Clear();
            return true;
        }

        return false;
    }

    protected abstract void CalculateVector(string barQualifier, StrategyCalculationContext context);
        
        
    protected void RegisterIndicator(string bsName, AbstractIndicator indicator, int lookback = 0, string? logName = null)
    {
        _skipBars[bsName] = Math.Max(_skipBars[bsName], _allBarStorages[bsName].RegisterIndicator(indicator, lookback));
    }

    protected void NewOrder(
        string symbol,
        decimal signedVolume,
        PositionEffect positionEffect = PositionEffect.Unknown,
        string? clOrdId = null,
        OrdType ordType = OrdType.Market,
        double? price = null,
        double? stopPx = null,
        bool isSuspended = false,
        string? positionId = null,
        IReadOnlyDictionary<string, LinkType> linkedOrders = null,
        bool isSLTP = false,
        PegInstructions? pegInstructions = null
    ) => NewOrder(symbol, signedVolume, positionEffect, clOrdId, ordType, price,
        stopPx, isSuspended, positionId, linkedOrders, isSLTP, pegInstructions, tradingSessionIds: _allBarStorages[symbol].BarStorageConfig.TradingSessionIds); // HACK, TODO: trading session ids are assigned only for single bar strategies
        

    protected void OpenPosition(
        string symbol,
        decimal signedVolume,
        double? stopLoss = null,
        double? takeProfit = null,
        string? positionId = null,
        decimal? trailingStopStep = null,
        OrdType ordType = OrdType.Market,
        double? price = null,
        int? orderValiditySeconds = null
    ) =>
        OpenPosition(symbol, signedVolume, stopLoss, takeProfit, positionId, trailingStopStep, ordType, price, 
            orderValiditySeconds: orderValiditySeconds, tradingSessionIds: _allBarStorages[symbol].BarStorageConfig.TradingSessionIds
        );        

        
    protected void ClosePosition(string symbol, decimal newFactor = 0, string? positionId = null, int? orderValiditySeconds = null)
    {
        var tradingSessions = _allBarStorages.TryGetValue(symbol, out var bs)
            ? bs.BarStorageConfig.TradingSessionIds
            : null;
        ClosePosition(symbol, newFactor, positionId, orderValiditySeconds: orderValiditySeconds,
            tradingSessionIds: tradingSessions);
    }
}