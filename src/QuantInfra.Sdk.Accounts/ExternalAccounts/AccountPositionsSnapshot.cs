using System.Collections.Generic;
using NodaTime;
using QuantInfra.Sdk.Trading.ExternalAccounts;

namespace QuantInfra.Sdk.Accounts.ExternalAccounts;

public class AccountPositionsSnapshot
{
    public int AccountId { get; set; }
    public List<ExternalPositionReport> Positions { get; set; }
    public Instant UpdateTs { get; set; }
}