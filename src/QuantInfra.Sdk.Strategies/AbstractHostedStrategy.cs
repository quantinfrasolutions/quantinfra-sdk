using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NodaTime;
using QuantInfra.Sdk.Accounting;
using QuantInfra.Sdk.Accounts;
using QuantInfra.Sdk.MarketData;
using QuantInfra.Sdk.StaticData;
using QuantInfra.Sdk.StaticData.Synthetics;
using QuantInfra.Sdk.Trading;
using QuantInfra.Sdk.Trading.Orders;
using QuantInfra.Sdk.Trading.Positions;

namespace QuantInfra.Sdk.Strategies;

/// <summary>
/// Base class for implementing strategies.
/// </summary>
public abstract class AbstractHostedStrategy
{
    public const int DefaultMarketOrderOpenPositionValiditySeconds = 120;
    public const string DefaultClOrdId = "0";
    public const string MainSymbolName = "main";

    protected AbstractHostedStrategy(Strategy s)
    {
        StrategyConfig = s;
        StrategyId = s.StrategyId;
    }
    
    public StrategyConfig StrategyConfig { get; }
    public int StrategyId { get; }
    
    
    protected IBarStorageProvider BarStoragesProvider { get; private set; } = null!;
    private IOrderBooksProvider _orderBooksProvider;
    private IBestBidOfferProvider _bestBidOfferProvider;
    private IHeartbeatsProvider _heartbeatsProvider;
    
    /// <summary>
    /// Default strategy logger
    /// </summary>
    protected ILogger Logger { get; private set; } = default!;
    protected ILoggerFactory LoggerFactory { get; private set; }

    
    #region Initialization helpers
    
    private readonly HashSet<int> _usedContractIds = new();
    public IReadOnlyCollection<int> UsedContractIds => _usedContractIds;
    private readonly Dictionary<int, string> _reverseContractsMapping = new();
    private readonly Dictionary<string, int> _contractsMapping = new();

    protected void ClaimContract(int contractId, string? name = null)
    {
        _usedContractIds.Add(contractId);
        if (!string.IsNullOrEmpty(name))
        {
            _reverseContractsMapping.TryAdd(contractId, name);
            _contractsMapping.TryAdd(name, contractId);
        }
    }

    private readonly Dictionary<string, IBarStorage> _barStorages = new();
    protected IReadOnlyDictionary<string, IBarStorage> BarStoragesByName => _barStorages;

    private readonly Dictionary<string, int> _orderBooksMap = new();
    protected IOrderBook? GetOrderBook(string name) => _orderBooksProvider[_orderBooksMap[name]];
    protected IOrderBook? GetOrderBook(int contractId) => _orderBooksProvider[contractId];
    
    private readonly Dictionary<string, int> _bboMap = new();
    protected BestBidOffer? GetBestBidOffer(string name) => _bestBidOfferProvider[_bboMap[name]];
    protected BestBidOffer? GetBestBidOffer(int contractId) => _bestBidOfferProvider[contractId];

    /// <summary>
    /// Call this method inside OnInitialize to request processing of heartbeats
    /// </summary>
    protected void ClaimHeartbeats()
    {
        var context = InitContext ??
                      throw new InvalidOperationException("ClaimHeartbeats can be called only from OnInitialize");
        _heartbeatsProvider.ClaimHeartbeats(StrategyId);
    }
    
    /// <summary>
    /// Call this method inside OnInitialize to request candles of a certain type.
    /// The returned BarStorage may be used to register indicators.
    ///
    /// After the bar storage is claimed, OnBarClosed will be called once the bar closes. 
    /// </summary>
    protected IBarStorage ClaimBarStorage(BarStorageConfig config, string? name = null)
    {
        var context = InitContext ??
            throw new InvalidOperationException("ClaimBarStorage can be called only from OnInitialize");
        
        if (config.IdType == IdType.Contract) _usedContractIds.Add(config.Id);
        var bs = BarStoragesProvider.ClaimBarStorage(config, context.GetContract, StrategyId);
        if (!string.IsNullOrEmpty(name)) _barStorages.Add(name, bs);
        return bs;
    }
    
    /// <summary>
    /// Call this method inside OnInitialize to request exchange bars for a certain contract.
    ///
    /// After the subscription is created, OnExchangeBar will be called upon new bar (basic aggregation unit, which is usually 1 minute) is received.
    /// </summary>
    protected void ClaimExchangeBars(IdType idType, int id, string? name = null, Period? lookback = null)
    {
        var context = InitContext ??
            throw new InvalidOperationException("ClaimExchangeBars can be called only from OnInitialize");
        
        if (idType == IdType.Contract) _usedContractIds.Add(id);
        BarStoragesProvider.ClaimExchangeBars(idType, id, context.GetContract, StrategyId, lookback);
    }

    /// <summary>
    /// Call this method inside OnInitialize to request exchange ticks for a certain contract.
    ///
    /// After the subscription is created, OnTick will be called upon new tick is received.
    /// </summary>
    protected void ClaimTicks(int contractId, string? name = null)
    {
        throw new NotImplementedException();
    }

    protected void ClaimOrderBook(int contractId, string? name = null)
    {
        var context = InitContext ??
                      throw new InvalidOperationException("ClaimOrderBook can be called only from OnInitialize");
        _orderBooksProvider.ClaimOrderBook(contractId, StrategyId);
        _usedContractIds.Add(contractId);
        
        if (!string.IsNullOrEmpty(name))
        {
            _orderBooksMap.Add(name, contractId);
        }
    }

    protected void ClaimBestBidOffer(int contractId, string? name = null)
    {
        var context = InitContext ??
                      throw new InvalidOperationException("ClaimBestBidOffer can be called only from OnInitialize");
        _bestBidOfferProvider.ClaimBestBidOffer(contractId, StrategyId);
        
        if (!string.IsNullOrEmpty(name))
        {
            _bboMap.Add(name, contractId);
        }
    }

    /// <summary>
    /// Call this method to ensure the strategy's account has the correct PositionAccounting. If not, an InvalidOperationException will be thrown.
    /// </summary>
    protected void RequireAccountingMode(PositionAccounting accounting)
    {
        var context = InitContext ??
            throw new InvalidOperationException("RequireAccountingMode can be called only from OnInitialize");
        
        if (context.Account.PositionAccounting != accounting)
            throw new InvalidOperationException($"Strategy requires position accounting to be set to {accounting}, actual is {InitContext.Account.PositionAccounting}");
    }

    /// <summary>
    /// Call this method to ensure that use of signal groups is configured properly. If not, an InvalidOperationException will be thrown.
    /// </summary>
    protected void RequireUsingSignalGroups(bool use)
    {
        if (InitContext is null)
            throw new InvalidOperationException("UseSignalGroups can be called only from OnInitialize");

        if (StrategyConfig.UseSignalGroups != use)
            throw new InvalidOperationException($"Strategy requires UseSignalGroups to be set to {use},  actual is {StrategyConfig.UseSignalGroups}");
    }
    
    #endregion
    
    internal StrategyInitializationContext? InitContext { get; private set; }
    internal HostedStrategyCalculationContext? Context { get; set; }

    private void CheckContext()
    {
        if (Context is null) throw new InvalidOperationException("Context is null");
    }
    
    internal void ClearContext() => Context = null;

    #region Handling methods
    
    internal void Initialize(StrategyInitializationContext context, IBarStorageProvider bsProvider, IOrderBooksProvider orderBooksProvider, 
        IBestBidOfferProvider bestBidOfferProvider, IHeartbeatsProvider heartbeatsProvider, ILoggerFactory loggerFactory)
    {
        InitContext = context;
        BarStoragesProvider = bsProvider;
        _orderBooksProvider = orderBooksProvider;
        _bestBidOfferProvider = bestBidOfferProvider;
        _heartbeatsProvider = heartbeatsProvider;
        Logger = loggerFactory.CreateLogger($"Strategy.{StrategyConfig.Name}.{StrategyId}");
        LoggerFactory = loggerFactory;
        
        // if (StrategyConfig.LiquidationParameters?.LiquidateAt == LiquidationTiming.OnBarClose)
        // {
        //     foreach (var s in StrategyConfig.Symbols.Values) ClaimExchangeBars(IdType.Contract, s);
        // }
        
        OnInitialize(InitContext!);
        InitContext = null;
    }

    internal void Deploy()
    {
        CheckContext();
        OnDeployed(Context!);
    }

    internal bool ProcessTick(ExchangeTrade tick)
    {
        CheckContext();
        return OnTick(tick, Context!);
    }

    internal bool ProcessHeartbeat()
    {
        CheckContext();
        OnHeartbeat(Context!);
        return true;
    }

    internal bool ProcessExchangeBar(ExchangeBar bar)
    {
        CheckContext();
        return OnExchangeBar(bar, Context!);
    }
    
    internal bool ProcessClosedBar(string barQualifier)
    {
        CheckContext();
        return OnBarClosed(barQualifier, Context!);
    }

    internal bool ProcessOrderbookL2Update(int contractId, IReadOnlyDictionary<decimal, decimal> updatedBids, IReadOnlyDictionary<decimal, decimal> updatedAsks)
    {
        CheckContext();
        return OnOrderBookUpdated(contractId, updatedBids, updatedAsks, Context!);
    }

    internal bool ProcessBestBidOfferUpdate(int contractId, BookLevel? bestBid, BookLevel? bestAsk)
    {
        CheckContext();
        return OnBestBidOfferUpdated(contractId, bestBid, bestAsk, Context!);
    }

    internal void ProcessExecutionReport(ExecutionReport er)
    {
        CheckContext();
        OnExecutionReport(er, Context!);
    }
    
    internal void ProcessTrade(Trade trade)
    {
        CheckContext();
        OnTrade(trade, Context!);
    }
    
    internal void ProcessEndOfDay(SharePriceHistory newSp)
    {
        CheckContext();
        OnEndOfDay(newSp, Context!);
    }
    
    internal void RollSyntheticContract(int contractId, CompositionUpdate composition)
    {
        CheckContext();
        OnSyntheticContractRolled(contractId, composition, Context!);
    }
    
    internal void ProcessInvestmentChange(decimal investment, decimal previousInvestment, decimal shareCountDelta, decimal newShareCount)
    {
        CheckContext();
        OnInvestmentChanged(investment, previousInvestment, shareCountDelta, newShareCount, Context!);
    }
    
    #endregion
    
    #region Callbacks

    /// <summary>
    /// Called when a strategy is activated (when the StrategiesService starts or when a new strategy is added and started).
    /// Use this method to claim market data and register indicators.
    /// </summary>
    /// <param name="initContext"></param>
    protected virtual void OnInitialize(StrategyInitializationContext initContext) { }

    /// <summary>
    /// Called immediately after the strategy is deployed (e.g., when the strategy is started or when the StrategiesService restarts)
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnDeployed(StrategyCalculationContext context) { }

    protected virtual void OnHeartbeat(StrategyCalculationContext context) { }
    
    /// <summary>
    /// Called when a tick is received
    /// </summary>
    protected virtual bool OnTick(ExchangeTrade tick, StrategyCalculationContext context) => context.IsHistory;

    /// <summary>
    /// Called when a 1-minute candle (or other basic aggregation unit, if so configured) is closed
    /// </summary>
    protected virtual bool OnExchangeBar(ExchangeBar bar, StrategyCalculationContext context) => context.IsHistory;

    /// <summary>
    /// Called when any of the bars requested by the strategy is closed
    /// </summary>
    protected virtual bool OnBarClosed(string barQualifier, StrategyCalculationContext context) => context.IsHistory;
    
    protected virtual bool OnOrderBookUpdated(int contractId, IReadOnlyDictionary<decimal, decimal> updatedBids, IReadOnlyDictionary<decimal, decimal> updatedAsks, StrategyCalculationContext context) => context!.IsHistory;
    
    protected virtual bool OnBestBidOfferUpdated(int contractId, BookLevel? bestBid, BookLevel? bestAsk,
        StrategyCalculationContext context) => context!.IsHistory;
    
    /// <summary>
    /// Called when a status of an order changes
    /// </summary>
    protected virtual void OnExecutionReport(ExecutionReport er, StrategyCalculationContext context) { }

    /// <summary>
    /// Called when a trade is booked
    /// </summary>
    protected virtual void OnTrade(Trade trade, StrategyCalculationContext context) { }
    
    /// <summary>
    /// Called after the composition of a synthetic contract traded by a strategy is changed and it is required to roll positions
    /// </summary>
    protected virtual void OnSyntheticContractRolled(long contractId, CompositionUpdate composition,
        StrategyCalculationContext context) { }

    /// <summary>
    /// Called after the positions are marked to market at the end of day
    /// </summary>
    protected virtual void OnEndOfDay(SharePriceHistory dailyReturn, StrategyCalculationContext context) { }

    /// <summary>
    /// Called when amount of funds allocated to the strategy is changed
    /// </summary>
    protected virtual void OnInvestmentChanged(decimal investment, decimal previousInvestment, decimal shareCountDelta,
        decimal newShareCount, StrategyCalculationContext context) { }

    #endregion
    
    
    #region Strategies API and helpers
    
    protected void UpdateInternalState(object? state)
    {
        var context = Context ?? throw new InvalidOperationException("UpdateInternalState can be called only from inside calculation methods");
        if (context.IsHistory) return;
        context.Strategy!.UpdateInternalState(state);
    }

    protected void NewOrder(
        string symbol,
        decimal signedVolume,
        PositionEffect positionEffect,
        string? clOrdId = null,
        OrdType type = OrdType.Market,
        double? price = null,
        double? stopPx = null,
        bool isSuspended = false,
        string? positionId = null,
        IReadOnlyDictionary<string, LinkType>? linkedOrders = null,
        bool isSltp = false,
        PegInstructions? pegInstructions = null,
        int? orderValiditySeconds = null,
        int[]? tradingSessionIds = null
    ) => NewOrder(GetContractId(symbol), signedVolume, positionEffect, clOrdId, type, price, stopPx, isSuspended,
        positionId, linkedOrders, isSltp, pegInstructions, orderValiditySeconds, tradingSessionIds);

    protected void NewOrder(
        int contractId,
        decimal signedVolume,            
        PositionEffect positionEffect,
        string? clOrdId = null,
        OrdType type = OrdType.Market,
        double? price = null,
        double? stopPx = null,
        bool isSuspended = false,
        string? positionId = null,
        IReadOnlyDictionary<string, LinkType>? linkedOrders = null,
        bool isSltp = false,
        PegInstructions? pegInstructions = null,
        int? orderValiditySeconds = null,
        int[]? tradingSessionIds = null
    )
    {
        var context = Context ?? throw new InvalidOperationException("NewOrder can be called only from inside calculation methods");
        
        if (!_usedContractIds.Contains(contractId))
        {
            throw new ArgumentException($"Contract {contractId} is not valid for the strategy. Call ClaimContract in OnInitialize to register the contract");
        }

        if (context.IsHistory) return;

        var positionAccounting = context.AccountRecord!.PositionAccounting;        
        if (positionAccounting == PositionAccounting.Hedged && positionEffect == PositionEffect.Unknown)
        {
            positionEffect = PositionEffect.Open;
        }
        
        #if !FAST
        Logger.LogInformation($"NewOrder: contractId={contractId}, contractId={contractId}, price={price}, signedVolume={signedVolume}, positionId={positionId}");
        #endif
        NewOrderSingle? order;

        var contract = context.GetContract(contractId); 
        var volume = Math.Abs(signedVolume);
        
        // In case of a real account, normalize the volume so a real order can be placed
        // For virtual accounts, just round to some fraction of a real contract size step
        volume = context.AccountRecord.AccountType == AccountType.VirtualAccount
            ? Math.Round(volume * context.VirtualAccountSizeStepFraction / contract.Template.SizeIncrement, 0, (MidpointRounding)2) 
                / context.VirtualAccountSizeStepFraction * contract.Template.SizeIncrement
            : contract.NormalizeVolume(volume);
        
        if (volume == 0)
        {
            if (context.ThrowOnZeroVolumeOrders)
                throw new NewOrderException(OrderExceptionReason.ZeroQuantity);
            return;
        }
        
        clOrdId ??= Guid.NewGuid().ToString();
        
        switch (type)
        {
            case OrdType.Market:
                if (price.HasValue)
                    throw new ArgumentException("Price cannot be submitted for a market order");
                if (stopPx.HasValue)
                    throw new ArgumentException("Stop price cannot be submitted for a market order");
                if (isSuspended)
                    throw new ArgumentException("Market order cannot be suspended");
                if (isSltp)
                    throw new ArgumentException("Market order cannot be SLTP");
                if (pegInstructions.HasValue)
                    throw new ArgumentException("Market order cannot be pegged");
                
                Instant? expireDt = positionEffect == PositionEffect.Open ? 
                        context.ReferenceDt.Plus(Duration.FromSeconds(orderValiditySeconds ?? DefaultMarketOrderOpenPositionValiditySeconds)) 
                        : orderValiditySeconds.HasValue 
                            ? context.ReferenceDt.Plus(Duration.FromSeconds(orderValiditySeconds.Value)) 
                            : null;
                
                order = NewOrderSingle.MarketOrder(clOrdId, context.AccountRecord.AccountId, contractId, positionId, positionEffect, 
                    volume, signedVolume.FromSign(), linkedOrders: linkedOrders, expireDt: expireDt, tradingSessionsIds: tradingSessionIds);
                
                break;

            case OrdType.Limit:
                if (!price.HasValue)
                    throw new ArgumentException("Price must be submitted for a limit order");
                if (stopPx.HasValue)
                    throw new ArgumentException("Stop price cannot be submitted for a limit order");
                if (pegInstructions.HasValue)
                    throw new ArgumentException("Limit order cannot be pegged");

                if (string.IsNullOrEmpty(clOrdId))
                {
                    if (isSltp)
                        clOrdId = GetTakeProfitClOrdId(positionId);
                    else
                        clOrdId = DefaultClOrdId;
                }

                order = NewOrderSingle.LimitOrder(
                    clOrdId,
                    context.AccountRecord.AccountId,
                    contractId,
                    positionId,
                    positionEffect,
                    volume,
                    signedVolume.FromSign(),
                    contract.NormalizePrice(price.Value),
                    linkedOrders: linkedOrders,
                    isSuspended: isSuspended,
                    isSLTP: isSltp,
                    expireDt: orderValiditySeconds.HasValue 
                        ? context.ReferenceDt.Plus(Duration.FromSeconds(orderValiditySeconds.Value))
                        : null, tradingSessionsIds: tradingSessionIds);
                break;

            case OrdType.StopMarket:
                if (!stopPx.HasValue)
                    throw new ArgumentException("Stop price must be submitted");
                if (price.HasValue)
                    throw new ArgumentException("Price cannot be submitted for a stop order");

                if (string.IsNullOrEmpty(clOrdId))
                {
                    if (isSltp)
                        clOrdId = GetStopLossClOrdId(positionId);
                    else
                        clOrdId = DefaultClOrdId;
                }

                order = NewOrderSingle.StopMarketOrder(
                    clOrdId,
                    context.AccountRecord.AccountId,
                    contractId,
                    positionId,
                    positionEffect,
                    volume,
                    signedVolume.FromSign(),
                    contract.NormalizePrice(stopPx.Value),
                    linkedOrders: linkedOrders,
                    isSuspended: isSuspended,
                    isSLTP: isSltp,
                    pegInstructions: pegInstructions,
                    expireDt: orderValiditySeconds.HasValue 
                        ? context.ReferenceDt.Plus(Duration.FromSeconds(orderValiditySeconds.Value))
                        : null, tradingSessionsIds: tradingSessionIds);
                break;

            case OrdType.StopLimit:
                if (!price.HasValue)
                    throw new ArgumentException("Price must be submitted for a stop limit order");
                if (!stopPx.HasValue)
                    throw new ArgumentException("Stop price must be submitted");
                if (isSltp)
                    throw new ArgumentException("Stop limit order cannot be SLTP");
                if (pegInstructions.HasValue)
                    throw new ArgumentException("Stop limit order cannot be pegged");

                order = NewOrderSingle.StopLimitOrder(
                    clOrdId ?? DefaultClOrdId,
                    context.AccountRecord.AccountId,
                    contractId,
                    positionId,
                    positionEffect,
                    volume,
                    signedVolume.FromSign(),
                    contract.NormalizePrice(price.Value),
                    contract.NormalizePrice(stopPx.Value),
                    linkedOrders: linkedOrders,
                    isSuspended: isSuspended,
                    expireDt: orderValiditySeconds.HasValue 
                        ? context.ReferenceDt.Plus(Duration.FromSeconds(orderValiditySeconds.Value))
                        : null, tradingSessionsIds: tradingSessionIds);
                break;

            case OrdType.MarketIfTouched:
                if (!price.HasValue)
                    throw new ArgumentException("Price must be submitted for a MIT order");
                if (stopPx.HasValue)
                    throw new ArgumentException("Stop price cannot be submitted for a MIT order");
                if (pegInstructions.HasValue)
                    throw new ArgumentException("MIT order cannot be pegged");
                if (isSltp)
                    throw new ArgumentException("MITorder cannot be SLTP");

                order = NewOrderSingle.MarketIfTouched(
                    clOrdId ?? DefaultClOrdId,
                    context.AccountRecord.AccountId,
                    contractId,
                    positionId,
                    positionEffect,
                    Math.Abs(signedVolume),
                    signedVolume.FromSign(),
                    contract.NormalizePrice(price.Value),
                    linkedOrders: linkedOrders,
                    isSuspended: isSuspended, 
                    tradingSessionsIds: tradingSessionIds
                );
                break;
            
            default: throw new ArgumentException($"Unsupported order type {type}");
        }

        if (!string.IsNullOrEmpty(order.ClOrdId) && context.AccountState!.Orders.Any(o => o.ClOrdId == order.ClOrdId))
        {
            throw new NewOrderException(OrderExceptionReason.DuplicateClOrdId);
        }
        
        context.Account!.PlaceOrder(order, context.ProcessingDt);
    }

    protected void NewOrder(NewOrderSingle order)
    {
        var context = Context ?? throw new InvalidOperationException("NewOrder can be called only from inside calculation methods");
        
        if (order.AccountId != context.AccountRecord!.AccountId) throw new Exception("AccountId mismatch");
        
        if (context.IsHistory) return;
        
        if (!string.IsNullOrEmpty(order.ClOrdId) && context.AccountState!.Orders.Any(o => o.ClOrdId == order.ClOrdId))
        {
            throw new NewOrderException(OrderExceptionReason.DuplicateClOrdId);
        }
        
        context.Account!.PlaceOrder(order, context.ProcessingDt);
    }
    
    protected void CancelOrder(string clOrdId)
    {
        var context = Context ?? throw new InvalidOperationException("CancelOrder can be called only from inside calculation methods");
        
        if (context.IsHistory) return;
        
        #if !FAST
        Logger.LogInformation($"CancelOrder: clOrdId={clOrdId}");
        #endif

        if (context.AccountState!.Orders.All(o => o.ClOrdId != clOrdId))
            throw new InvalidOperationException($"Order {clOrdId} does not exist");
        context.Account!.CancelOrder(new() { AccountId = context.AccountRecord!.AccountId, OrigClOrdId = clOrdId }, context.ProcessingDt);
    }
    
    protected void CancelOrder(long orderId)
    {
        var context = Context ?? throw new InvalidOperationException("CancelOrder can be called only from inside calculation methods");
        
        if (context.IsHistory) return;
        
        #if !FAST
        Logger.LogInformation($"CancelOrder: orderId={orderId}");
        #endif

        if (orderId == 0)
            throw new InvalidOperationException($"Use ClOrdId to cancel orders that do not have order id assigned");
        
        if (context.AccountState!.Orders.All(o => o.OrderId != orderId))
            throw new InvalidOperationException($"Order {orderId} does not exist");
        context.Account!.CancelOrder(new() { AccountId = context.AccountRecord.AccountId, OrderId = orderId }, context.ProcessingDt);
    }
    
    protected void ReplaceOrder(OrderReplaceRequest request)
    {
        var context = Context ?? throw new InvalidOperationException("ReplaceOrder can be called only from inside calculation methods");
        
        if (context.IsHistory) return;
        
        context.Account!.ReplaceOrder(request, context.ProcessingDt);
    }

    /// <summary>
    /// Opens new position using a market order
    /// </summary>
    protected void OpenPosition(
        string symbol,
        decimal signedVolume,
        double? stopLoss = null,
        double? takeProfit = null,
        string? positionId = null,
        decimal? trailingStopStep = null,
        OrdType ordType = OrdType.Market,
        double? price = null,
        double? stopPx = null,
        int? orderValiditySeconds = null,
        int[]? tradingSessionIds = null
    ) => OpenPosition(GetContractId(symbol), signedVolume, stopLoss, takeProfit, positionId, trailingStopStep,
        ordType, price, stopPx, orderValiditySeconds, tradingSessionIds);
    
    /// <summary>
    /// Opens new position using a market order
    /// </summary>
    protected void OpenPosition(
        int contractId,
        decimal signedVolume,
        double? stopLoss = null,
        double? takeProfit = null,
        string? positionId = null,
        decimal? trailingStopStep = null,
        OrdType ordType = OrdType.Market,
        double? price = null,
        double? stopPx = null,
        int? orderValiditySeconds = null,
        int[]? tradingSessionIds = null
    )
    {
        if (!_usedContractIds.Contains(contractId))
            throw new ArgumentException($"Contract {contractId} is not valid for the strategy. Call ClaimContract in OnInitialize to register the contract");
        
#if !FAST
        Logger.LogInformation($"OpenPosition: contractId={contractId}, contractId={contractId}, signedVolume={signedVolume}, positionId={positionId}, stopLoss={stopLoss}, takeProfit={takeProfit}");
#endif
        var slClOrdId = GetStopLossClOrdId(positionId);
        var tpClOrdId = GetTakeProfitClOrdId(positionId);
        
        NewOrder(contractId, signedVolume, PositionEffect.Open, GetOpenPositionClOrdId(positionId), ordType, positionId: positionId,
           linkedOrders: CreateLinkedOrdersList((slClOrdId, LinkType.OneTriggersOther), (tpClOrdId, LinkType.OneTriggersOther)),
           price: price, stopPx: stopPx, orderValiditySeconds: orderValiditySeconds, tradingSessionIds: tradingSessionIds);

        if (stopLoss != null)
        {
            NewOrder(contractId, -signedVolume, PositionEffect.Close, slClOrdId, OrdType.StopMarket,
                stopPx: stopLoss, isSuspended: true, isSltp: true, positionId: positionId,
                linkedOrders: CreateLinkedOrdersList((tpClOrdId, LinkType.OneCancelsOther)),
                // TODO: supports only existing type of trailing stop
                pegInstructions: trailingStopStep.HasValue ?
                    new PegInstructions(trailingStopStep.Value, PegMoveType.Approaching) :
                    null,
                tradingSessionIds: tradingSessionIds
            );
        }

        if (takeProfit != null)
        {
            NewOrder(contractId, -signedVolume, PositionEffect.Close, tpClOrdId, OrdType.Limit,
                price: takeProfit, isSuspended: true, isSltp: true, positionId: positionId,
                linkedOrders: CreateLinkedOrdersList((slClOrdId, LinkType.OneCancelsOther)),
                tradingSessionIds: tradingSessionIds
            );
        }
    }

    /// <summary>
    /// Reduces a position _to_ the desired volume
    /// </summary>
    protected void ClosePosition(string symbol, decimal newVolume = 0, string? positionId = null, string? clOrdId = null, int? orderValiditySeconds = null, int[]? tradingSessionIds = null)
    {
        var context = Context ?? throw new InvalidOperationException("ClosePosition can be called only from inside calculation methods");
        
        if (newVolume != 0)
        {
            throw new NotSupportedException("Partial closing a position is not yet supported");
            // Partial closing of a position requires updating the volumes of SLTP
        }
        var contractId = GetContractId(symbol);
#if !FAST
        Logger.LogInformation($"ClosePosition: symbol={symbol}, contractId={contractId}, newVolume={newVolume}, positionId={positionId}");
#endif            
        var position = context.AccountState.Positions.SingleOrDefault(p => p.ContractId == contractId && p.StrategyPositionId == positionId);
        if (position is null) throw new InvalidOperationException($"Position {positionId} not found in contract {symbol}");
        var closeVolume = Math.Max(position.Volume - newVolume, 0);
        if (closeVolume != 0)
        {
            var slClOrdId = GetStopLossClOrdId(positionId);
            var tpClOrdId = GetTakeProfitClOrdId(positionId);
            NewOrder(symbol, closeVolume * position.Side.Invert().GetSign(), PositionEffect.Close, clOrdId,
                positionId: positionId,
                linkedOrders: CreateLinkedOrdersList((slClOrdId, LinkType.OneCancelsOther),
                    (tpClOrdId, LinkType.OneCancelsOther)),
                orderValiditySeconds: orderValiditySeconds,
                tradingSessionIds: tradingSessionIds
            );
        }
    }

    public void ChangePositionSl(string symbol, double? price = null, string? positionId = null)
    {
        var context = Context ?? throw new InvalidOperationException("ChangePositionSL can be called only from inside calculation methods");
        
        var contractId = GetContractId(symbol);
#if !FAST
        Logger.LogInformation($"ChangePositionSL: symbol={symbol}, contractId={contractId}, price={price}, positionId={positionId}");
#endif
        var contract = context.GetContract(contractId); 
        
        var clOrdId = GetStopLossClOrdId(positionId);
        var order = context.AccountState.Orders.SingleOrDefault(o =>
            o.ContractId == contractId && o.ClOrdId == clOrdId);
        
        if (order != null)
        {
            if (price == null)
            {
                CancelOrder(clOrdId);
            }
            else
            {
                ReplaceOrder(new OrderReplaceRequest
                {
                    AccountId = context.AccountState.AccountId,
                    OrderId = order.OrderId,
                    StopPx = contract.NormalizePrice(price.Value),
                });   
            }
        }
        else if (price != null)
        {
            var position = GetPosition(contractId, positionId) ?? throw new InvalidOperationException($"Position {positionId} in {symbol} does not exist");
            NewOrder(symbol, -position.SignedVolume, PositionEffect.Close, clOrdId, OrdType.StopMarket,
                stopPx: price, isSltp: true, positionId: positionId,
                linkedOrders: CreateLinkedOrdersList((GetTakeProfitClOrdId(positionId), LinkType.OneCancelsOther))
            );
        }
        // TODO: Trailing stops
    }

    protected void CancelPositionSl(string symbol, string? positionId = null)
        => ChangePositionSl(symbol, null, positionId);

    protected void ChangePositionTp(string symbol, double? price = null, string? positionId = null)
    {
        var context = Context ?? throw new InvalidOperationException("ChangePositionTP can be called only from inside calculation methods");
        
        var contractId = GetContractId(symbol);
        #if !FAST
        Logger.LogInformation($"ChangePositionTP: contractId={symbol}, price={price}, positionId={positionId}");
        #endif
        
        var contract = context.GetContract(contractId); 
        
        var clOrdId = GetStopLossClOrdId(positionId);
        var order = GetOrder(contractId, clOrdId, true);
        
        if (order != null)
        {
            if (price == null)
            {
                CancelOrder(clOrdId);
            }
            else
            {
                ReplaceOrder(new OrderReplaceRequest
                {
                    AccountId = context.AccountState.AccountId,
                    OrderId = order.OrderId,
                    Price = contract.NormalizePrice(price.Value),
                });
            }
        }
        else if (price != null)
        {
            var position = GetPosition(contractId, positionId) ?? throw new InvalidOperationException($"Position {positionId} in {symbol} does not exist");
            NewOrder(symbol, -position.SignedVolume, PositionEffect.Close, clOrdId, OrdType.Limit, price: price, positionId: positionId,
                linkedOrders: CreateLinkedOrdersList((GetStopLossClOrdId(positionId), LinkType.OneCancelsOther)));
        }
    }

    protected void CancelPositionTp(string symbol, string? positionId = null)
        => ChangePositionTp(symbol, null, positionId);

    
    public int GetContractId(string symbol = MainSymbolName) => _contractsMapping[symbol];

    protected Position? GetPosition(string symbol = MainSymbolName, string? strategyPositionId = null) =>
        GetPosition(GetContractId(symbol), strategyPositionId);


    protected IEnumerable<Position> GetPositions() =>
        throw new NotSupportedException();
        // Context?.AccountState.Positions
        // ?? throw new InvalidOperationException("GetPositions can be called only from inside calculation methods");
    
    protected Position? GetPosition(int contractId, string? strategyPositionId) =>
        throw new NotSupportedException();
        // GetPositions().GetPosition(contractId, strategyPositionId);
    
    public IEnumerable<Position> GetPositionsByContract(int contractId) =>
        throw new NotSupportedException();
        // GetPositions().GetPositionsByContractId(contractId);

    protected IEnumerable<Position> GetPositionsByContract(string symbol = MainSymbolName) =>
        throw new NotSupportedException();
        // GetPositionsByContract(GetContractId(symbol));
    
    protected bool IsInPosition(string symbol = MainSymbolName) => throw new NotSupportedException();
        // IsInPosition(GetContractId(symbol));

    protected bool IsInPosition(int contractId)
        => throw new NotSupportedException();//GetPositionsByContract(contractId).Any();

    protected int GetNumberOfPositions(string symbol = MainSymbolName) =>
        throw new NotSupportedException();
        // GetPositionsByContract(symbol).Count();

    protected IEnumerable<OrderStatus> GetOrdersByContract(string symbol) =>
        throw new NotSupportedException();
        // GetOrdersByContract(GetContractId(symbol));

    protected IEnumerable<OrderStatus> GetOrders() =>
        throw new NotSupportedException();
        // Context?.AccountState.Orders
        // ?? throw new InvalidOperationException("GetOrders can be called only from inside calculation methods");
    
    protected IEnumerable<OrderStatus> GetOrdersByContract(int contractId) =>
        throw new NotSupportedException();
        // GetOrders().GetOrdersByContractId(contractId);

    protected OrderStatus? GetOrder(int contractId, string clOrdId, bool includeSuspended = false) =>
        throw new NotSupportedException();
        // GetOrders().GetOrder(contractId, clOrdId, includeSuspended);

    public OrderStatus? GetPositionStopLoss(string symbol, string positionId, bool includeSuspended = false) =>
        throw new NotSupportedException();
        // GetOrder(GetContractId(symbol), GetStopLossClOrdId(positionId), includeSuspended);
    
    public OrderStatus? GetPositionStopLoss(int contractId, string positionId, bool includeSuspended = false) =>
        throw new NotSupportedException();
        // GetOrder(contractId, GetStopLossClOrdId(positionId), includeSuspended);

    protected OrderStatus? GetPositionTakeProfit(string symbol, string positionId, bool includeSuspended = false) =>
        throw new NotSupportedException();
        // GetOrder(GetContractId(symbol), GetTakeProfitClOrdId(positionId), includeSuspended);
    
    protected OrderStatus? GetPositionTakeProfit(int contractId, string positionId, bool includeSuspended = false) =>
        throw new NotSupportedException();
        // GetOrder(contractId, GetTakeProfitClOrdId(positionId), includeSuspended);
    
    private string GetOpenPositionClOrdId(string positionId) => $"{positionId}-o";
    private string GetStopLossClOrdId(string positionId) => $"{positionId}-sl";
    private string GetTakeProfitClOrdId(string positionId) => $"{positionId}-tp";
    private string GetClosePositionClOrdId(string positionId) => $"{positionId}-c";
    
    private IReadOnlyDictionary<int, decimal>? _baseVolumesCache;
    //
    protected decimal GetVolume(int contractId, decimal value)
    {
        var context = Context ?? throw new InvalidOperationException("GetVolume can be called only from inside calculation methods");
        
        if (value == 0) return 0;
        
        var lastPrice = BarStoragesProvider.GetLastPrice(contractId) ?? 0;
        if (lastPrice == 0) return 0;
        
        var contract = Context!.GetContract(contractId);
        return contract.GetCalculator().GetVolume(value, (decimal)lastPrice);
        
        // if (context.AccountRecord.AccountType == AccountType.VirtualAccount)
        // {
        //     return Math.Round(vol, context.VirtualAccountSizeStepFraction);
        // }
        // else
        // {
        //     return contract.NormalizeVolume(Math.Abs(vol)) * Math.Sign(vol);
        // }
    }
    
    protected decimal GetVolume(string symbol, decimal value) => GetVolume(GetContractId(symbol), value);
    
    
    public static IReadOnlyDictionary<string, LinkType> CreateLinkedOrdersList(params (string, LinkType)[] orders) =>
        orders.ToDictionary(o => o.Item1, o => o.Item2);
    
    #endregion
    
    // private void OnProcessingFinish()
    // {
    //     // _context = null;
    //     _baseVolumesCache = null;
    // }
}