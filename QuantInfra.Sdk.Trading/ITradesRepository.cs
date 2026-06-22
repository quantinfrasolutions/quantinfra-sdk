namespace QuantInfra.Sdk.Trading
{
    public interface ITradesRepository
    {
        void RequestLastTrades(string internalAccountId);
    }
}
