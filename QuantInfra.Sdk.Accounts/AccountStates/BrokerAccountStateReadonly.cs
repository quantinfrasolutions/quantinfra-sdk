using System.Collections.Generic;
using System.Text.Json.Serialization;
using Common.Trading.Positions;
using NodaTime;
using QuantInfra.Sdk.Trading.ExternalAccounts;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Accounts.AccountStates;

// TODO: move to another repo
public class BrokerAccountStateReadonly : AccountStateReadonly, IBrokerAccountStateReadonly
{
    [JsonConstructor]
    public BrokerAccountStateReadonly(
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
        long version, 
        Instant lastReconciliationDt, 
        Instant lastReceivedTradeTs, 
        IReadOnlyCollection<string> lastReceivedTradeIds, 
        IReadOnlyDictionary<long, ExecutionReport> pendingFills, 
        Instant lastReceivedBalanceOperationTs, 
        IReadOnlyCollection<string> lastReceivedBalanceOperationIds, 
        IReadOnlyCollection<ExternalTradeRecord> tradesDeadLetterQueue, 
        IReadOnlyCollection<string> unmappedExternalContractIds,
        IReadOnlyDictionary<string, Instant> usedContractIds, 
        bool isReconciled,
        bool needsOrdersReconciliation,
        bool needsTradesReconciliation
    ) : base(accountServiceName, accountId, positionAccounting, balances, positions, orders, investment, 
        sharePrice, shareCount, hwm, realizedPnLSinceLastMtm, version)
    {
        LastReconciliationDt = lastReconciliationDt;
        LastReceivedTradeTs = lastReceivedTradeTs;
        LastReceivedTradeIds = lastReceivedTradeIds;
        PendingFills = pendingFills;
        LastReceivedBalanceOperationTs = lastReceivedBalanceOperationTs;
        LastReceivedBalanceOperationIds = lastReceivedBalanceOperationIds;
        TradesDeadLetterQueue = tradesDeadLetterQueue;
        UnmappedExternalContractIds = unmappedExternalContractIds;
        UsedContractIds = usedContractIds;
        IsReconciled = isReconciled;
        NeedsOrdersReconciliation = needsOrdersReconciliation;
        NeedsTradesReconciliation = needsTradesReconciliation;
    }


    public Instant LastReconciliationDt { get; }
    public Instant LastReceivedTradeTs { get; }
    public IReadOnlyCollection<string> LastReceivedTradeIds { get; }
    public IReadOnlyDictionary<long, ExecutionReport> PendingFills { get; }
    public Instant LastReceivedBalanceOperationTs { get; }
    public IReadOnlyCollection<string> LastReceivedBalanceOperationIds { get; }
    public IReadOnlyCollection<ExternalTradeRecord> TradesDeadLetterQueue { get; }
    public IReadOnlyCollection<string> UnmappedExternalContractIds { get; }
    public IReadOnlyDictionary<string, Instant> UsedContractIds { get; }
    public bool IsReconciled { get; }
    public bool NeedsOrdersReconciliation { get; }
    public bool NeedsTradesReconciliation { get; }
}