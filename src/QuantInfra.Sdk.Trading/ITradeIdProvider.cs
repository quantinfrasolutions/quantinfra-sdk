namespace QuantInfra.Sdk.Trading;

public interface ITradeIdProvider
{
    long GetNextTradeId();
}