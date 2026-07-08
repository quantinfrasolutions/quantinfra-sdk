using System;
using System.Collections.Generic;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.StaticData;

public enum BrokerType
{
    Ibkr = 1,
    BinanceUsdmFutures = 2,
}

public interface IBroker
{
    IReadOnlyCollection<OrdType> SupportedOrderTypes { get; }
}


// TODO: move to connectors
public class BinanceUsdmFutures : IBroker
{
    private static readonly HashSet<OrdType> _supportedOrderTypes = new() { OrdType.Market, OrdType.Limit, };

    public IReadOnlyCollection<OrdType> SupportedOrderTypes => _supportedOrderTypes;
}

public class IBKR : IBroker
{
    private static readonly HashSet<OrdType> _supportedOrderTypes = new() { OrdType.Market, OrdType.Limit, };

    public IReadOnlyCollection<OrdType> SupportedOrderTypes => _supportedOrderTypes;
}

public static class Brokers
{
    private static IBroker _binanceUsdmFutures = new BinanceUsdmFutures();
    private static IBroker _ibkr = new IBKR();
    public static IBroker GetBroker(BrokerType brokerType) => brokerType switch
    {
        BrokerType.Ibkr => _ibkr,
        BrokerType.BinanceUsdmFutures => _binanceUsdmFutures,
        _ => throw new ArgumentOutOfRangeException()
    };

}