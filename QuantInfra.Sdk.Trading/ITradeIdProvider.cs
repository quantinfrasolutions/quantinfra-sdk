namespace Common.Trading;

public interface ITradeIdProvider
{
    long GetNextTradeId();
}