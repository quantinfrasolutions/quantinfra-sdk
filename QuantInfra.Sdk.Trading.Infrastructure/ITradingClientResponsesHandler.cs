using QuantInfra.Sdk.Accounts.ExternalAccounts;
using QuantInfra.Sdk.Trading.ExternalAccounts;

namespace QuantInfra.Sdk.Trading.Infrastructure;

public interface ITradingClientResponsesHandler
{
    void OnConnect(int accountId);
    /// <param name="receivedAt">Unix timestamp of message reception. Use SystemClock.Instance.GetCurrentInstant().ToUnixTimeMilliseconds()</param>
    /// <param name="swReceivedAt">Stopwatch timestamp of message reception. Use MetricsUtils.GetUnixMicro() helper</param>
    void OnExecutionReport(ExternalExecutionReport er, long receivedAt, long swReceivedAt);
    /// <param name="receivedAt">Unix timestamp of message reception. Use SystemClock.Instance.GetCurrentInstant().ToUnixTimeMilliseconds()</param>
    /// <param name="swReceivedAt">Stopwatch timestamp of message reception. Use MetricsUtils.GetUnixMicro() helper</param>
    void OnOrderCancelReject(ExternalOrderCancelReject ocr, long receivedAt, long swReceivedAt);
    /// <param name="receivedAt">Unix timestamp of message reception. Use SystemClock.Instance.GetCurrentInstant().ToUnixTimeMilliseconds()</param>
    /// <param name="swReceivedAt">Stopwatch timestamp of message reception. Use MetricsUtils.GetUnixMicro() helper</param>
    void OnTrade(ExternalTradeRecord trade, long receivedAt, long swReceivedAt);
    /// <param name="receivedAt">Unix timestamp of message reception. Use SystemClock.Instance.GetCurrentInstant().ToUnixTimeMilliseconds()</param>
    /// <param name="swReceivedAt">Stopwatch timestamp of message reception. Use MetricsUtils.GetUnixMicro() helper</param>
    void OnBalanceOperation(ExternalBalanceOperation bo, long receivedAt, long swReceivedAt);
    /// <param name="receivedAt">Unix timestamp of message reception. Use SystemClock.Instance.GetCurrentInstant().ToUnixTimeMilliseconds()</param>
    /// <param name="swReceivedAt">Stopwatch timestamp of message reception. Use MetricsUtils.GetUnixMicro() helper</param>
    void OnOrdersSnapshotReceived(int accountId, bool success, ExternalAccountOrdersSnapshot? snapshot, long receivedAt, long swReceivedAt);
    /// <param name="receivedAt">Unix timestamp of message reception. Use SystemClock.Instance.GetCurrentInstant().ToUnixTimeMilliseconds()</param>
    /// <param name="swReceivedAt">Stopwatch timestamp of message reception. Use MetricsUtils.GetUnixMicro() helper</param>
    void OnFullSnapshotReceived(int accountId, bool success, ExternalAccountFullSnapshot? snapshot, long receivedAt, long swReceivedAt);
}