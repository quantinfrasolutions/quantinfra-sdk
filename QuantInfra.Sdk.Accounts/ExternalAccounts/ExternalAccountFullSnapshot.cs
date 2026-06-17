using System.Collections.Generic;
using NodaTime;
using QuantInfra.Sdk.Trading.ExternalAccounts;

namespace QuantInfra.Sdk.Accounts.ExternalAccounts;

public class ExternalAccountFullSnapshot
{
    public int AccountId { get; init; }
    public Instant UpdateTs { get; init; }
    
    public IReadOnlyCollection<ExternalExecutionReport> Orders { get; init; }
    public IReadOnlyCollection<ExternalTradeRecord> Trades { get; init; }
    public IReadOnlyCollection<ExternalPositionReport> Positions { get; init; }
    public IReadOnlyDictionary<string, decimal> Balances { get; init; }
}