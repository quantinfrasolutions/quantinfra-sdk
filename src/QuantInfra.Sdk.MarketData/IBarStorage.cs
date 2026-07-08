using NodaTime;

namespace QuantInfra.Sdk.MarketData;

public interface IBarStorage : IEnumerable<IBar>
{
    string FullQualifier { get; }
    BarStorageConfig BarStorageConfig { get; }
    int StreamId { get; }
    
    IBar CurrentBar { get; }
    int Count { get; }
    IBar this[int i] { get; }
    
    Instant? AppendBar(ExchangeBar bar);
    
    void InitiateStorage(int capacity);
    int RegisterIndicator(AbstractIndicator indicator, int lookback = 0);
    
    int GetBarsCountSinceMoment(Instant dt);
}