namespace QuantInfra.Sdk.Trading;

public interface IPositionIdProvider
{
    long GetNextPositionId();
    long GetNextPositionHistoryId();
}