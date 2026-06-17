using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Common.Trading.Positions;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Accounts.AccountStates;

// TODO: move to another repo
public class AccountStateReadonly : IAccountStateReadonly
{
    [JsonConstructor]
    public AccountStateReadonly(
        string accountServiceName,
        int accountId,
        PositionAccounting positionAccounting,
        IReadOnlyDictionary<int, decimal> balances,
        IEnumerable<Position> positions,
        IEnumerable<OrderStatus> orders,
        decimal investment,
        decimal sharePrice,
        decimal shareCount,
        decimal hwm,
        decimal realizedPnLSinceLastMtm,
        long version
    )
    {
        AccountServiceName = accountServiceName;
        AccountId = accountId;
        PositionAccounting = positionAccounting;
        Balances = balances.ToDictionary(kv => kv.Key, kv => kv.Value);
        Positions = positions.ToList();
        Orders = orders.ToList();
        Investment = investment;
        SharePrice = sharePrice;
        ShareCount = shareCount;
        HWM = hwm;
        RealizedPnLSinceLastMtm = realizedPnLSinceLastMtm;
        Version = version;
    }

    public string AccountServiceName { get; }
    public int AccountId { get; }
    public PositionAccounting PositionAccounting { get; }
    public IReadOnlyDictionary<int, decimal> Balances { get; }
    public IEnumerable<Position> Positions { get; }
    public IEnumerable<OrderStatus> Orders { get; }
    public IEnumerable<ExecutionReport> OrdersHistory { get; }
    public decimal Investment { get; }
    public decimal SharePrice { get; }
    public decimal ShareCount { get; }
    public decimal HWM { get; }
    public decimal RealizedPnLSinceLastMtm { get; }
    public long Version { get; }
}