using NodaTime;
using QuantInfra.Sdk.StaticData;

namespace QuantInfra.Sdk.MarketData;

public interface IBarStorageProvider
{
    IBarStorage this[string fullQualifier] { get; }
        
    IBarStorage ClaimBarStorage(BarStorageConfig config, Func<int, Contract> getContract,
        int? strategyId = null);
        
    IAggregatingBarStorage CreateBarStorage(BarStorageConfig config, Func<int, Contract> getContract, int? strategyId = null);
        
    void ClaimExchangeBars(IdType idType, int id, Func<int, Contract> getContract, int strategyId,
        Period? lookback);

    double? GetLastPrice(int contractId);
}