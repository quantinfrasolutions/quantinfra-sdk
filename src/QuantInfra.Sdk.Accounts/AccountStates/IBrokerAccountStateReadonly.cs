using System.Collections.Generic;
using NodaTime;
using QuantInfra.Sdk.Trading.ExternalAccounts;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Accounts.AccountStates;

public interface IBrokerAccountStateReadonly : IAccountStateReadonly
{
    Instant LastReconciliationDt { get; }
    Instant LastReceivedTradeTs { get; }
    IReadOnlyCollection<string> LastReceivedTradeIds { get; }
    IReadOnlyDictionary<long, ExecutionReport> PendingFills { get; }
    Instant LastReceivedBalanceOperationTs { get; }
    IReadOnlyCollection<string> LastReceivedBalanceOperationIds { get; }
    IReadOnlyCollection<ExternalTradeRecord> TradesDeadLetterQueue { get; }
    IReadOnlyCollection<string> UnmappedExternalContractIds { get; }
    IReadOnlyDictionary<string, Instant> UsedContractIds { get; }
    bool IsReconciled { get; }
    bool NeedsOrdersReconciliation { get; }
    bool NeedsTradesReconciliation { get; }
}