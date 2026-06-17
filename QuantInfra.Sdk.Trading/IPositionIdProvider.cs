namespace Common.Trading;

public interface IPositionIdProvider
{
    long GetNextPositionId();
    long GetNextPositionHistoryId();
}