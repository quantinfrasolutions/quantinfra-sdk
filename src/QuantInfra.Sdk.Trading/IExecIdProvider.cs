namespace QuantInfra.Sdk.Trading;

public interface IExecIdProvider
{
    long GetNextExecId();
}