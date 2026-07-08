using NodaTime;
using QuantInfra.Sdk.Accounting;

namespace QuantInfra.Sdk.Trading.ExternalAccounts;

public class ExternalBalanceOperation
{
    public ExternalBalanceOperation(int accountId, string externalAssetId, decimal balanceChange, string externalId, Instant dt,
        bool isSwap = false, string? swapPositionExternalContractId = null)
    {
        AccountId = accountId;
        ExternalAssetId = externalAssetId;
        BalanceChange = balanceChange;
        ExternalId = externalId;
        Dt = dt;
        IsSwap = isSwap;
        SwapPositionExternalContractId = swapPositionExternalContractId;
    }

    public int AccountId { get; init; }
    public string ExternalAssetId { get; }
    public decimal BalanceChange { get; }
    public string ExternalId { get; }
    public Instant Dt { get; }
    public bool IsSwap { get; }
    public string? SwapPositionExternalContractId { get; }

    public NewBalanceOperation ToNewBalanceOperation(int assetId, bool affectsBalance, bool affectsPnL, bool affectsInvestment) => new()
    {
        AccountId = AccountId,
        Dt = Dt,
        Amount = BalanceChange,
        AssetId = assetId,
        ExternalId = ExternalId,
        AffectsPnL = affectsPnL,
        AffectsBalance = affectsBalance,
        AffectsInvestment = affectsInvestment
    };
}