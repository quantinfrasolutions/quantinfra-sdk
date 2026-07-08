namespace QuantInfra.Sdk.MarketData;

public interface IAggregatingBarStorage : IBarStorage
{
    ExchangeBar CurrentAggregation { get; }
}