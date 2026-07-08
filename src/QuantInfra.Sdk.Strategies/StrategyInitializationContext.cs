using QuantInfra.Sdk.Accounts;
using QuantInfra.Sdk.StaticData;

namespace QuantInfra.Sdk.Strategies;

public abstract class StrategyInitializationContext
{
    public StrategyInitializationContext(AccountRecordV6 account)
    {
        Account = account;
    }
    
    public AccountRecordV6 Account { get; }
    public abstract Contract GetContract(int contractId);
}