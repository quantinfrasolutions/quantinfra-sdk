using Microsoft.Extensions.Hosting;
using NodaTime;

namespace QuantInfra.Sdk.Trading.Infrastructure;

public interface IHostedTradingClient : ITradingClient, IHostedService
{
    bool IsConnected();
    void RequestAccountOrdersSnapshot(Guid? requestId = null);
    void RequestAccountFullSnapshot(IReadOnlyDictionary<string, Instant>? lastReceivedTradeDts, Instant? lastReceivedBalanceOperationDt, Guid? requestId = null);
}