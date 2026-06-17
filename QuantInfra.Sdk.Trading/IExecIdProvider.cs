namespace Common.Trading;

public interface IExecIdProvider
{
    long GetNextExecId();
}