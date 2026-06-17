using System.Collections.Generic;
using NodaTime;
using QuantInfra.Sdk.Trading.ExternalAccounts;

namespace QuantInfra.Sdk.Accounts.ExternalAccounts;

public class ExternalAccountOrdersSnapshot
{
    public int AccountId { get; init; }
    public IReadOnlyCollection<ExternalExecutionReport> Orders { get; init; }
    public Instant UpdateTs { get; init; }
}