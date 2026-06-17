namespace QuantInfra.Sdk.Accounts;

public class AccountRecordV6
{
    public AccountRecordV6() { }
    
    public AccountRecordV6(string accountServiceName, string name, int currencyId, AccountType accountType,
        PositionAccounting positionAccounting, int? brokerId, bool enableSharePriceTracking,
        bool includeUnrealizedPnLToMtm, TradingClientConfig? tradingClientConfig, int accountId = 0)
    {
        AccountServiceName = accountServiceName;
        Name = name;
        CurrencyId = currencyId;
        AccountType = accountType;
        PositionAccounting = positionAccounting;
        BrokerId = brokerId;
        EnableSharePriceTracking = enableSharePriceTracking;
        IncludeUnrealizedPnLToMtm = includeUnrealizedPnLToMtm;
        AccountId = accountId;
    }
    
    public AccountRecordV6(AccountRecordV6 a)
    {
        AccountId = a.AccountId;
        AccountServiceName = a.AccountServiceName;
        Name = a.Name;
        CurrencyId = a.CurrencyId;
        AccountType = a.AccountType;
        PositionAccounting = a.PositionAccounting;
        BrokerId = a.BrokerId;
        EnableSharePriceTracking = a.EnableSharePriceTracking;
        TradingClientConfig = a.TradingClientConfig;
    }

    public int AccountId { get; init; }
    public string AccountServiceName { get; init; }
    public string Name { get; init; }
    public int CurrencyId { get; init; }
    public AccountType AccountType { get; init; }
    public PositionAccounting PositionAccounting { get; init; }
    public int? BrokerId { get; init; }
    public bool EnableSharePriceTracking { get; init; }
    public bool IncludeUnrealizedPnLToMtm { get; init; }
    public TradingClientConfig? TradingClientConfig { get; init; }
}