using System.Collections.Generic;
using NodaTime;

namespace QuantInfra.Sdk.Accounts.ExternalAccounts;

public class AccountBalancesSnapshot
{
    public int AccountId { get; init; }
    public IReadOnlyDictionary<string, decimal> Balances { get; init; }
    public Instant UpdateTs { get; init; }
}