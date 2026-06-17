using System;
namespace Common.Trading
{
    public interface ITradesRepository
    {
        void RequestLastTrades(string internalAccountId);
    }
}
