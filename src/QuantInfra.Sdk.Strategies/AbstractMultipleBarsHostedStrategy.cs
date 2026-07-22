using System;
using System.Collections.Generic;
using System.Linq;
using QuantInfra.Sdk.MarketData;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Strategies;

public abstract class AbstractMultipleBarsHostedStrategy<TParams> : AbstractHostedStrategy<TParams>
    where TParams : class, new()
{
    private readonly Dictionary<string, int> _skipBars = new();
    private readonly Dictionary<string, string> _bsCommonToLocal = new();

    public AbstractMultipleBarsHostedStrategy(Strategy sc) : base(sc)
    {
            
    }

    private readonly Dictionary<string, IBarStorage> _allBarStorages = new();
    private readonly Dictionary<string, IBarStorage> _currentCycleBarStorages = new();
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

    protected override void OnBarClosed(string barQualifier, StrategyCalculationContext context)
    {
        var localName = _bsCommonToLocal[barQualifier];
        _currentCycleBarStorages[localName] = _allBarStorages[localName];
            
        if (_currentCycleBarStorages.Count == _allBarStorages.Count)
        {
            if (_currentCycleBarStorages.Keys.Any(bs => _currentCycleBarStorages[bs].Count < _skipBars[bs])) return;
            CalculateVector(barQualifier, context);
            _currentCycleBarStorages.Clear();
        }
    }

    protected abstract void CalculateVector(string barQualifier, StrategyCalculationContext context);
        
        
    protected void RegisterIndicator(string bsName, AbstractIndicator indicator, int lookback = 0, string? logName = null)
    {
        _skipBars[bsName] = Math.Max(_skipBars[bsName], _allBarStorages[bsName].RegisterIndicator(indicator, lookback));
    }
}