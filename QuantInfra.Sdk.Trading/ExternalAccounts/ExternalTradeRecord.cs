using System.Text.Json.Serialization;
using Common.Trading;
using NodaTime;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Trading.ExternalAccounts;

public class ExternalTradeRecord
{
    protected ExternalTradeRecord() { }
    
    [JsonConstructor]
    public ExternalTradeRecord(string externalTradeId, string externalOrderId, string externalContractId,
        int accountId, Side side, decimal volume, decimal price, decimal commission, string? commissionCurrency,
        Instant dt, decimal calculatedCcyLastQty)
    {
        AccountId = accountId;
        ExternalContractId = externalContractId;
        Side = side;
        Volume = volume;
        Price = price;
        Commission = commission;
        CommissionCurrency = commissionCurrency;
        Dt = dt;
        ExternalOrderId = externalOrderId;
        ExternalTradeId = externalTradeId;
        CalculatedCcyLastQty = calculatedCcyLastQty;
    }
    
    public ExternalTradeRecord(ExternalTradeRecord t) : this(t.ExternalTradeId, t.ExternalOrderId, t.ExternalContractId,
        t.AccountId, t.Side, t.Volume, t.Price, t.Commission, t.CommissionCurrency, t.Dt, t.CalculatedCcyLastQty
    ) { }

    public int AccountId { get; init; }
    public string ExternalContractId { get; init; }
    public Side Side { get; init; }
    public decimal Volume { get; init; }
    public decimal Price { get; init; }
    public decimal Commission { get; init; }
    public Instant Dt { get; init; }
    public string ExternalOrderId { get; init; }
    public string ExternalTradeId { get; init; }
    public string? CommissionCurrency { get; set; }
    public decimal CalculatedCcyLastQty { get; init; }

    public decimal SignedVolume => Volume * Side.GetSign();

    public Trade ToTrade(string accountServiceName, long tradeId, int contractId, string? clOrdId, long? orderId, long? execId, 
        string? strategyPositionId, long? executionRequestId, int paymentCcyId, decimal fxRate, decimal? calculatedLastCcyOverride = null,
        PositionEffect? positionEffect = null)
    {
        return new Trade(
            accountServiceName,
            tradeId,
            clOrdId,
            AccountId,
            contractId,
            orderId, 
            execId,
            strategyPositionId,
            null,
            positionEffect,
            Side,
            Volume,
            Price,
            Commission,
            Dt,
            executionRequestId,
            ExternalTradeId,
            null,
            paymentCcyId, fxRate,
            calculatedLastCcyOverride ?? CalculatedCcyLastQty,
            null, 
            null,
            false
        );
    }

    public override string ToString()
    {
        return $"{{ ExternalTradeRecord | AccountId: {AccountId}, ExternalContractId: {ExternalContractId}, Side: {Side}, Volume: {Volume}, Price: {Price}, Commission: {Commission}, Dt: {Dt}, ExternalOrderId: {ExternalOrderId}, ExternalTradeId: {ExternalTradeId}, CommissionCurrency: {CommissionCurrency} }}";
    }
}