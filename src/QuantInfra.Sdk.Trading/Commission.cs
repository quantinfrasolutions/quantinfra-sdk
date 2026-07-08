namespace QuantInfra.Sdk.Trading;

public class Commission
{
    public int CommissionId { get; init; }
    public long TradeId { get; init; }
    public decimal Amount { get; init; }
}