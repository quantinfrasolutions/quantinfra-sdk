namespace QuantInfra.Sdk.MarketData;

public interface IHeartbeatsProvider
{
    void ClaimHeartbeats(int? strategyId = null);
}