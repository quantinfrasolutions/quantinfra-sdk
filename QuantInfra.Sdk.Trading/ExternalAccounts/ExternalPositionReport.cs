using NodaTime;

namespace QuantInfra.Sdk.Trading.ExternalAccounts;

public class ExternalPositionReport
{
    public ExternalPositionReport() { }
    
    public ExternalPositionReport(int accountId, string externalContractId, decimal signedVolume, decimal openPrice, Instant? openDt)
    {
        AccountId = accountId;
        ExternalContractId = externalContractId;
        SignedVolume = signedVolume;
        OpenPrice = openPrice;
        OpenDt = openDt;
    }

    public int AccountId { get; init; }
    public string ExternalContractId { get; init; }
    public decimal SignedVolume { get; init; }
    public decimal OpenPrice { get; init; }
    public Instant? OpenDt { get; init; }
    public long BrokerId { get; init; }

    public override string ToString() =>
        $"{{ExternalPositionReport | AccountId: {AccountId}, ExternalContractId: {ExternalContractId}, SignedVolume: {SignedVolume}, OpenPrice: {OpenPrice}, OpenDt: {OpenDt}, BrokerId: {BrokerId} }}";
}