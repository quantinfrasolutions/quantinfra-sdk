using System;

namespace QuantInfra.Sdk.StaticData;

public class CommissionStructure
{
	public int CommissionId { get; init; }
	public string Name { get; init; } = string.Empty;
	public string? Description { get; init; }
	public decimal FixedPerShare { get; init; }
	public decimal Floating { get; init; }
	public Currency? Currency { get; init; } = null!;
	public CommissionStructureType CommissionStructureType { get; init; }
	public int? BrokerId { get; init; }
	public int? ExchangeId { get; init; }
		
	public decimal GetCommission(decimal tradeQty, decimal tradeValue, int? decimalsOverride = null) =>
		Math.Round(Floating * tradeValue + FixedPerShare * tradeQty, decimalsOverride ?? Currency.Decimals);
}