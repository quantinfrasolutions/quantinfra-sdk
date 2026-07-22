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

    internal void ProcessTick(ExchangeTrade tick)
    {
        CheckContext();
        OnTick(tick, Context!);
    }

    internal bool ProcessHeartbeat()
    {
        CheckContext();
        OnHeartbeat(Context!);
        return true;
    }

    internal void ProcessExchangeBar(ExchangeBar bar)
    {
        CheckContext();
        OnExchangeBar(bar, Context!);
    }
    
    internal void ProcessClosedBar(string barQualifier)
    {
        CheckContext();
        OnBarClosed(barQualifier, Context!);
    }

    internal void ProcessOrderbookL2Update(int contractId, IReadOnlyDictionary<decimal, decimal> updatedBids, IReadOnlyDictionary<decimal, decimal> updatedAsks)
    {
        CheckContext();
        OnOrderBookUpdated(contractId, updatedBids, updatedAsks, Context!);
    }

    internal void ProcessBestBidOfferUpdate(int contractId, BookLevel? bestBid, BookLevel? bestAsk)
    {
        CheckContext();
        OnBestBidOfferUpdated(contractId, bestBid, bestAsk, Context!);
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
    protected virtual void OnTick(ExchangeTrade tick, StrategyCalculationContext context) { }

    /// <summary>
    /// Called when a 1-minute candle (or other basic aggregation unit, if so configured) is closed
    /// </summary>
    protected virtual void OnExchangeBar(ExchangeBar bar, StrategyCalculationContext context) { }

    /// <summary>
    /// Called when any of the bars requested by the strategy is closed
    /// </summary>
    protected virtual void OnBarClosed(string barQualifier, StrategyCalculationContext context) { }
    
    protected virtual void OnOrderBookUpdated(int contractId, IReadOnlyDictionary<decimal, decimal> updatedBids, IReadOnlyDictionary<decimal, decimal> updatedAsks, StrategyCalculationContext context)
    { }
    
    protected virtual void OnBestBidOfferUpdated(int contractId, BookLevel? bestBid, BookLevel? bestAsk,
        StrategyCalculationContext context)
    { }
    
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

    public void NewOrder(NewOrderSingle order)
    {
        var context = Context 
            ?? throw new InvalidOperationException("NewOrder can be called only from inside calculation methods");
        
        if (order.AccountId != context.AccountRecord!.AccountId) throw new Exception("AccountId mismatch");
        
        if (context.IsHistory) return;
        
        if (!string.IsNullOrEmpty(order.ClOrdId) && context.AccountState!.Orders.Any(o => o.ClOrdId == order.ClOrdId))
        {
            throw new NewOrderException(OrderExceptionReason.DuplicateClOrdId);
        }
        
        context.Account!.PlaceOrder(order, context.ProcessingDt);
    }
    
    public void CancelOrder(string clOrdId)
    {
        var context = Context 
            ?? throw new InvalidOperationException("CancelOrder can be called only from inside calculation methods");
        
        if (context.IsHistory) return;
        
        Logger.LogInformation("CancelOrder: clOrdId={clOrdId}", clOrdId);

        if (context.AccountState!.Orders.All(o => o.ClOrdId != clOrdId))
            throw new InvalidOperationException($"Order {clOrdId} does not exist");
        context.Account!.CancelOrder(new() { AccountId = context.AccountRecord!.AccountId, OrigClOrdId = clOrdId }, context.ProcessingDt);
    }
    
    public void CancelOrder(long orderId)
    {
        var context = Context 
            ?? throw new InvalidOperationException("CancelOrder can be called only from inside calculation methods");
        
        if (context.IsHistory) return;
        
        Logger.LogInformation("CancelOrder: orderId={orderId}", orderId);

        if (orderId <= 0)
            throw new InvalidOperationException($"Use ClOrdId to cancel orders that do not have order id assigned");
        
        if (context.AccountState!.Orders.All(o => o.OrderId != orderId))
            throw new InvalidOperationException($"Order {orderId} does not exist");
        context.Account!.CancelOrder(new() { AccountId = context.AccountRecord.AccountId, OrderId = orderId }, context.ProcessingDt);
    }
    
    public void ReplaceOrder(OrderReplaceRequest request)
    {
        var context = Context 
            ?? throw new InvalidOperationException("ReplaceOrder can be called only from inside calculation methods");
        
        if (request.OrderId.HasValue && request.OrderId <= 0)
            throw new InvalidOperationException($"Use ClOrdId to replace orders that do not have order id assigned");
        
        if (context.IsHistory) return;
        
        context.Account!.ReplaceOrder(request, context.ProcessingDt);
    }
    
    /// <summary>
    /// Opens new position
    /// </summary>
    public void OpenPosition(
        string symbol,
        decimal signedVolume,
        double? stopLoss = null,
        double? takeProfit = null,
        string? positionId = null,
        OrdType ordType = OrdType.Market,
        double? price = null,
        double? stopPx = null,
        int? orderValiditySeconds = null
    ) => OpenPosition(GetContractId(symbol), signedVolume, stopLoss, takeProfit, positionId,
        ordType, price, stopPx, orderValiditySeconds);
    
    /// <summary>
    /// Opens new position
    /// </summary>
    public void OpenPosition(
        int contractId,
        decimal signedVolume,
        double? stopLoss = null,
        double? takeProfit = null,
        string? positionId = null,
        OrdType ordType = OrdType.Market,
        double? price = null,
        double? stopPx = null,
        int? orderValiditySeconds = null
    )
    {
        var context = Context 
            ?? throw new InvalidOperationException("OpenPosition can be called only from inside calculation methods");
        
        if (!_usedContractIds.Contains(contractId))
            throw new ArgumentException($"Contract {contractId} is not valid for the strategy. Call ClaimContract in OnInitialize to register the contract");
        
        Logger.LogInformation("OpenPosition: contractId={contractId}, signedVolume={signedVolume}, positionId={positionId}, stopLoss={stopLoss}, takeProfit={takeProfit}",
            contractId, signedVolume, positionId, stopLoss, takeProfit);

        var openClOrdId = GetOpenPositionClOrdId(positionId);
        var slClOrdId = GetStopLossClOrdId(positionId);
        var tpClOrdId = GetTakeProfitClOrdId(positionId);
        var contract = context.GetContract(contractId)!;
        var expireDt = orderValiditySeconds.HasValue
            ? (Instant?)context.ReferenceDt.Plus(Duration.FromSeconds(orderValiditySeconds.Value))
            : null;

        var nos = ordType switch
        {
            OrdType.Market => NewOrderSingle.MarketOrder(openClOrdId, context.AccountRecord.AccountId,
                contractId, positionId, PositionEffect.Open, Math.Abs(signedVolume), signedVolume.FromSign(),
                linkedOrders: CreateLinkedOrdersList((slClOrdId, LinkType.OneTriggersOther), (tpClOrdId, LinkType.OneTriggersOther)),
                expireDt: expireDt
            ),
            OrdType.Limit => NewOrderSingle.LimitOrder(openClOrdId, context.AccountRecord.AccountId,
                contractId, positionId, PositionEffect.Open, Math.Abs(signedVolume), signedVolume.FromSign(), contract.NormalizePrice(price!.Value),
                linkedOrders: CreateLinkedOrdersList((slClOrdId, LinkType.OneTriggersOther), (tpClOrdId, LinkType.OneTriggersOther)),
                expireDt: expireDt
            ),
            OrdType.StopMarket => NewOrderSingle.StopMarketOrder(openClOrdId, context.AccountRecord.AccountId,
                contractId, positionId, PositionEffect.Open, Math.Abs(signedVolume), signedVolume.FromSign(), contract.NormalizePrice(stopPx!.Value),
                linkedOrders: CreateLinkedOrdersList((slClOrdId, LinkType.OneTriggersOther), (tpClOrdId, LinkType.OneTriggersOther))
            ),
            OrdType.StopLimit => NewOrderSingle.StopLimitOrder(openClOrdId, context.AccountRecord.AccountId,
                contractId, positionId, PositionEffect.Open, Math.Abs(signedVolume), signedVolume.FromSign(), 
                contract.NormalizePrice(price!.Value), contract.NormalizePrice(stopPx!.Value),
                linkedOrders: CreateLinkedOrdersList((slClOrdId, LinkType.OneTriggersOther), (tpClOrdId, LinkType.OneTriggersOther)),
                expireDt: expireDt
            ),
            OrdType.MarketIfTouched => NewOrderSingle.MarketIfTouched(openClOrdId, context.AccountRecord.AccountId,
                contractId, positionId, PositionEffect.Open, Math.Abs(signedVolume), signedVolume.FromSign(), contract.NormalizePrice(price!.Value),
                linkedOrders: CreateLinkedOrdersList((slClOrdId, LinkType.OneTriggersOther), (tpClOrdId, LinkType.OneTriggersOther)),
                expireDt: expireDt
            ),
            _ => throw new ArgumentException(nameof(ordType))
        };
        
        NewOrder(nos);

        if (stopLoss != null)
        {
            NewOrder(NewOrderSingle.StopMarketOrder(slClOrdId, context.AccountRecord.AccountId, 
                contractId, positionId, PositionEffect.Close, Math.Abs(signedVolume), signedVolume.FromSign().Invert(),
                contract.NormalizePrice(stopLoss.Value),
                linkedOrders: CreateLinkedOrdersList((tpClOrdId, LinkType.OneCancelsOther))
            ));
        }

        if (takeProfit != null)
        {
            NewOrder(NewOrderSingle.LimitOrder(slClOrdId, context.AccountRecord.AccountId, 
                contractId, positionId, PositionEffect.Close, Math.Abs(signedVolume), signedVolume.FromSign().Invert(),
                contract.NormalizePrice(takeProfit.Value),
                linkedOrders: CreateLinkedOrdersList((slClOrdId, LinkType.OneCancelsOther))
            ));
        }
    }
    
    /// <summary>
    /// Closes the position using a market order
    /// </summary>
    public void ClosePosition(int contractId, string? positionId = null, string? clOrdId = null)
    {
        var context = Context 
            ?? throw new InvalidOperationException("ClosePosition can be called only from inside calculation methods");
        
        Logger.LogInformation("ClosePosition: contractId={contractId}, positionId={positionId}",
            contractId, positionId);
          
        var position = context.AccountState.Positions.GetPosition(contractId, positionId);
        if (position is null) return;

        if (string.IsNullOrEmpty(clOrdId)) clOrdId = GetClosePositionClOrdId(positionId);
        var slClOrdId = GetStopLossClOrdId(positionId);
        var tpClOrdId = GetTakeProfitClOrdId(positionId);
        NewOrder(NewOrderSingle.MarketOrder(
            clOrdId,
            context.AccountRecord.AccountId,
            contractId,
            positionId,
            PositionEffect.Close,
            position.Volume,
            position.Side.Invert(),
            linkedOrders: CreateLinkedOrdersList((slClOrdId, LinkType.OneCancelsOther), (tpClOrdId, LinkType.OneCancelsOther))
        ));
    }
    
    /// <summary>
    /// Closes the position using a market order
    /// </summary>
    protected void ClosePosition(string symbol, string? positionId = null, string? clOrdId = null) =>
        ClosePosition(GetContractId(symbol), positionId, clOrdId);
    
    /// <summary>
    /// Creates, updates, or cancels the stop loss order for the position 
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="price">Pass null to cancel the order</param>
    /// <param name="positionId"></param>
    public void ChangePositionSl(int contractId, double? price = null, string? positionId = null)
    {
        var context = Context 
            ?? throw new InvalidOperationException("ChangePositionSL can be called only from inside calculation methods");

        Logger.LogInformation("ChangePositionSL: contractId={contractId}, price={price}, positionId={positionId}",
            contractId, price, positionId);
        
        var contract = context.GetContract(contractId)!;
        var order = GetPositionStopLoss(contractId, positionId);
        
        if (order != null)
        {
            if (price == null)
            {
                CancelOrder(order.OrderId);
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
            var position = context.AccountState.Positions.GetPosition(contractId, positionId) 
                ?? throw new InvalidOperationException($"Position {positionId} with contractId {contractId} does not exist");
            NewOrder(NewOrderSingle.StopMarketOrder(
                GetStopLossClOrdId(positionId),
                context.AccountRecord.AccountId,
                contractId,
                positionId,
                PositionEffect.Close,
                position.Volume,
                position.Side.Invert(),
                contract.NormalizePrice(price.Value),
                isSLTP: true,
                linkedOrders: CreateLinkedOrdersList((GetTakeProfitClOrdId(positionId), LinkType.OneCancelsOther))
            ));
        }
        // TODO: Trailing stops
    }
    
    /// <summary>
    /// Creates, updates, or cancels the stop loss order for the position 
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="price">Pass null to cancel the order</param>
    /// <param name="positionId"></param>
    public void ChangePositionSl(string symbol, double? price = null, string? positionId = null) =>
        ChangePositionSl(GetContractId(symbol), price, positionId);

    /// <summary>
    /// Cancels position stop loss order if it exists
    /// </summary>
    /// <param name="contractId"></param>
    /// <param name="positionId"></param>
    public void CancelPositionSl(int contractId, string? positionId = null)
        => ChangePositionSl(contractId, null, positionId);
    
    /// <summary>
    /// Cancels position stop loss order if it exists
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="positionId"></param>
    public void CancelPositionSl(string symbol, string? positionId = null)
        => ChangePositionSl(symbol, null, positionId);

    /// <summary>
    /// Creates, updates, or cancels the take profit order for the position 
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="price">Pass null to cancel the order</param>
    /// <param name="positionId"></param>
    public void ChangePositionTp(int contractId, double? price = null, string? positionId = null)
    {
        var context = Context 
            ?? throw new InvalidOperationException("ChangePositionTP can be called only from inside calculation methods");
        
        Logger.LogInformation("ChangePositionTP: contractId={contractId}, price={price}, positionId={positionId}",
            contractId, price, positionId);
        
        var contract = context.GetContract(contractId)!;
        var order = GetPositionTakeProfit(contractId, positionId);
        
        if (order != null)
        {
            if (price == null)
            {
                CancelOrder(order.OrderId);
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
            var position = context.AccountState.Positions.GetPosition(contractId, positionId) 
                ?? throw new InvalidOperationException($"Position {positionId} with contractId {contractId} does not exist");
            NewOrder(NewOrderSingle.LimitOrder(
                GetStopLossClOrdId(positionId),
                context.AccountRecord.AccountId,
                contractId,
                positionId,
                PositionEffect.Close,
                position.Volume,
                position.Side.Invert(),
                contract.NormalizePrice(price.Value),
                isSLTP: true,
                linkedOrders: CreateLinkedOrdersList((GetStopLossClOrdId(positionId), LinkType.OneCancelsOther))
            ));
        }
    }
    
    /// <summary>
    /// Creates, updates, or cancels the take profit order for the position 
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="price">Pass null to cancel the order</param>
    /// <param name="positionId"></param>
    public void ChangePositionTp(string symbol, double? price = null, string? positionId = null) =>
        ChangePositionTp(GetContractId(symbol), price, positionId);

    /// <summary>
    /// Cancels take profit order for the position, if it exists
    /// </summary>
    /// <param name="contractId"></param>
    /// <param name="positionId"></param>
    public void CancelPositionTp(int contractId, string? positionId = null)
        => ChangePositionTp(contractId, null, positionId);
    
    /// <summary>
    /// Cancels take profit order for the position, if it exists
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="positionId"></param>
    public void CancelPositionTp(string symbol, string? positionId = null)
        => ChangePositionTp(symbol, null, positionId);

    
    public int GetContractId(string symbol) => _contractsMapping[symbol];
    
    public string GetOpenPositionClOrdId(string symbol, string? positionId = null) => $"{symbol}-{positionId}-o";
    public string GetClosePositionClOrdId(string symbol, string? positionId = null) => $"{symbol}-{positionId}-c";
    public string GetStopLossClOrdId(string symbol, string? positionId = null) => $"{symbol}-{positionId}-sl";
    public string GetTakeProfitClOrdId(string symbol, string? positionId = null) => $"{symbol}-{positionId}-tp";

    public OrderStatus? GetPositionStopLoss(int contractId, string? positionId = null)
    {
        var context = Context 
            ?? throw new InvalidOperationException("GetPositionStopLoss can be called only from inside calculation methods");
        
        var clOrdId = GetStopLossClOrdId(positionId);
        return context.AccountState.Orders.SingleOrDefault(o =>
            o.ContractId == contractId && o.ClOrdId == clOrdId);
    }
    
    public OrderStatus? GetPositionStopLoss(string symbol, string? positionId = null) =>
        GetPositionStopLoss(GetContractId(symbol), positionId);
    
    public OrderStatus? GetPositionTakeProfit(int contractId, string? positionId = null)
    {
        var context = Context 
            ?? throw new InvalidOperationException("GetPositionTakeProfit can be called only from inside calculation methods");
        
        var clOrdId = GetTakeProfitClOrdId(positionId);
        return context.AccountState.Orders.SingleOrDefault(o =>
            o.ContractId == contractId && o.ClOrdId == clOrdId);
    }
    
    public OrderStatus? GetPositionTakeProfit(string symbol, string? positionId = null) =>
        GetPositionTakeProfit(GetContractId(symbol), positionId);
    
    
    /// <summary>
    /// Calculates the volume given the desired value and price
    /// </summary>
    /// <param name="contractId"></param>
    /// <param name="value">Defaults to the investment of the account</param>
    /// <param name="price">Defaults to the price</param>
    /// <param name="normalize">Whether to align the volume with the contract's MinSize and SizeStep</param>
    /// <exception cref="InvalidOperationException"></exception>
    public decimal GetVolume(int contractId, decimal? value = null, decimal? price = null, bool normalize = true)
    {
        var context = Context 
            ?? throw new InvalidOperationException("GetVolume can be called only from inside calculation methods");

        value ??= context.AccountState.Investment;
        if (value == 0) return 0;
        
        price ??= (decimal?)BarStoragesProvider.GetLastPrice(contractId) ?? 0m;
        if (price == 0) return 0;
        
        var contract = Context!.GetContract(contractId)!;
        var vol = contract.GetCalculator().GetVolume(value.Value, price.Value);
        return normalize ? contract.NormalizeVolume(vol) : vol;
    }
    
    /// <summary>
    /// Calculates the volume given the desired value and price
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="value">Defaults to the investment of the account</param>
    /// <param name="price">Defaults to the price</param>
    /// <param name="normalize">Whether to align the volume with the contract's MinSize and SizeStep</param>
    /// <exception cref="InvalidOperationException"></exception>
    public decimal GetVolume(string symbol, decimal? value = null, decimal? price = null, bool normalize = true) 
        => GetVolume(GetContractId(symbol), value, price, normalize);
    
    /// <summary>
    /// Returns the list that can be passed to the NewOrderSingle constructor
    /// </summary>
    /// <param name="orders">ClOrdId + LinkType of the linked orders</param>
    public static IReadOnlyDictionary<string, LinkType> CreateLinkedOrdersList(params (string, LinkType)[] orders) =>
        orders.ToDictionary(o => o.Item1, o => o.Item2);
    
    #endregion
}