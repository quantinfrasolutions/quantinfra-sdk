using System.Collections.Generic;
using Common.Trading.Positions;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Accounts.AccountStates;

public interface IAccountStateReadonly
{
    string AccountServiceName { get; }
    int AccountId { get; }
    PositionAccounting PositionAccounting { get; }
    
    /// <summary>
    /// Cash balances by currency
    /// </summary>
    IReadOnlyDictionary<int, decimal> Balances { get; }
    /// <summary>
    /// Active positions
    /// </summary>
    IEnumerable<Position> Positions { get; }
    /// <summary>
    /// Active orders
    /// </summary>
    IEnumerable<OrderStatus> Orders { get; }
    /// <summary>
    /// Current investment
    /// </summary>
    decimal Investment { get; }
    /// <summary>
    /// Current share price
    /// </summary>
    decimal SharePrice { get; }
    /// <summary>
    /// Current share count
    /// </summary>
    decimal ShareCount { get; }
    /// <summary>
    /// Share price high watermark
    /// </summary>
    decimal HWM { get; }
    decimal RealizedPnLSinceLastMtm { get; }
    long Version { get; }
}