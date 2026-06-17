namespace QuantInfra.Sdk.Accounts;

public class Subaccount
{
    public Subaccount() { }
    
    public Subaccount(int subaccountHistoryId, int accountId, int subaccountId, SubaccountType classifier, int? brokerId)
    {
        SubaccountHistoryId = subaccountHistoryId;
        AccountId = accountId;
        SubaccountId = subaccountId;
        Classifier = classifier;
        BrokerId = brokerId;
    }
    
    public Subaccount(Subaccount other) : this(other.SubaccountHistoryId, other.AccountId, other.SubaccountId, other.Classifier, other.BrokerId) { }

    public int SubaccountHistoryId { get; init; }
    public int AccountId { get; init; }
    public int SubaccountId { get; init; }
    public SubaccountType Classifier { get; init; }
    
    public int? BrokerId { get; init; }
}

public enum SubaccountType
{
    Broker,
}