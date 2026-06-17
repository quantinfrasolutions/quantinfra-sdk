using NodaTime;

namespace QuantInfra.Sdk.Accounting;

public class SharePriceHistory
{
    public int AccountId { get; init; }
    public Instant Dt { get; init; }
    public decimal ShareCount { get; init; }
    public decimal SharePrice { get; init; }
    public decimal DailyReturn { get; init; }
    public decimal HWM { get; init; }
    public decimal Investment { get; set; }
    public SharePriceHistoryChangeType Type { get; init; }

    public SharePriceHistory() { }

    public SharePriceHistory(SharePriceHistory sp)
    {
        AccountId = sp.AccountId;
        Dt = sp.Dt;
        SharePrice = sp.SharePrice;
        DailyReturn = sp.DailyReturn;
        HWM = sp.HWM;
        Investment = sp.Investment;
        Type = sp.Type;
    }

    public SharePriceHistory(int accountId, Instant dt, decimal sharePrice, decimal shareCount, decimal dailyReturn, decimal hwm, decimal investment, SharePriceHistoryChangeType type)
    {
        AccountId = accountId;
        Dt = dt;
        SharePrice = sharePrice;
        ShareCount = shareCount;
        DailyReturn = dailyReturn;
        HWM = hwm;
        Investment = investment;
        Type = type;
    }

    public override string ToString()
    {
        return
            $"{nameof(Dt)}: {Dt}, {nameof(ShareCount)}: {ShareCount}, {nameof(SharePrice)}: {SharePrice}, {nameof(DailyReturn)}: {DailyReturn}, {nameof(HWM)}: {HWM}, {nameof(Investment)}: {Investment}, {nameof(Type)}: {Type}";
    }
}

public enum SharePriceHistoryChangeType
{
    EndOfDay,
    BalanceOperation
}