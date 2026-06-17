using System.Collections.Generic;
using NodaTime;
using QuantInfra.Sdk.Trading.ExternalAccounts;

namespace QuantInfra.Sdk.Accounts.ExternalAccounts;

public class ExternalAccountTradesReport
{
    public int AccountId { get; init; }
    public long BrokerId { get; init; }
    public List<ExternalTradeRecord> Trades { get; init; }
    public Instant UpdateTs { get; set; }
}