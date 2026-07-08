using NodaTime;

namespace QuantInfra.Sdk.Accounting;

public class BalanceValue
{
    public BalanceValue(int accountId,
        int currencyId,
        Instant dt,
        decimal cashBalance,
        decimal holdings,
        decimal unrealizedPnL,
        decimal futuresVariationMargin,
        decimal totalBalance,
        decimal totalValue,
        decimal fxRate
    )
    {
        AccountId = accountId;
        CurrencyId = currencyId;
        Dt = dt;
        CashBalance = cashBalance;
        Holdings = holdings;
        UnrealizedPnL = unrealizedPnL;
        FuturesVariationMargin = futuresVariationMargin;
        TotalBalance = totalBalance;
        TotalValue = totalValue;
        FxRate = fxRate;
    }

    public BalanceValue(BalanceValue other) : this(other.AccountId, other.CurrencyId, other.Dt, other.CashBalance,
        other.Holdings, other.UnrealizedPnL, other.FuturesVariationMargin, other.TotalBalance, other.TotalValue,
        other.FxRate)
    {
        
    }
    
    public int AccountId { get; }
    public int CurrencyId { get; init; }
    public Instant Dt { get; init; }
    /// <summary>
    /// Deposits + withdrawals + commissions + funding/interest payments + spot trade payments 
    /// </summary>
    public decimal CashBalance { get; init; }
    /// <summary>
    /// Balance value (as of position opening) of holdings in Stock contracts with settlement currency equal to CurrencyId
    /// </summary>
    public decimal Holdings { get; init; }
    /// <summary>
    /// Unrealized PnL of Stock contracts
    /// </summary>
    public decimal UnrealizedPnL { get; init; }
    /// <summary>
    /// Current variation margin of futures with settlement currency equal to CurrencyId
    /// </summary>
    public decimal FuturesVariationMargin { get; init; }
    /// <summary>
    /// CashBalance + Holdings + UnrealizedPnL + FuturesVariationMargin
    /// </summary>
    public decimal TotalBalance { get; init; }
    /// <summary>
    /// TotalBalance converted to account currency
    /// </summary>
    public decimal TotalValue { get; init; }
    public decimal FxRate { get; init; }
}