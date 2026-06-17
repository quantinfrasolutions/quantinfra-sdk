namespace QuantInfra.Sdk.MarketData;

public interface IActiveSubscriptionsRepository<TSubscriptionRequest, TSubscription, TSubscriptionView>
{
    Task AddSubscriptionAsync(TSubscription subscription);
    Task<IReadOnlyCollection<TSubscriptionView>> GetActiveSubscriptionsAsync(string clientName);
    Task RemoveSubscriptionAsync(int id);
    Task UpdateSubscriptionStreamAsync(int id, long? streamId);
}