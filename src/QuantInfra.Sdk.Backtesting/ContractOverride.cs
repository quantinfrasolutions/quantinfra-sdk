using QuantInfra.Sdk.StaticData;
using QuantInfra.Sdk.Trading;

namespace QuantInfra.Sdk.Backtesting;

public class ContractOverride
{
    public bool OverrideAllContracts { get; init; } = false;
    public SecurityType SecurityType { get; init; } = SecurityType.Stock;
    public PnLCalculatorType PnLCalculatorType { get; init; } = PnLCalculatorType.Default;
    public int SettlementCurrencyDecimals { get; init; } = 2;
    public decimal CostPerShare { get; init; } = 0m;
    public decimal FloatingCost { get; init; } = 0m;
    public decimal TickSize { get; init; } = 0.01m;
    public decimal TickValue { get; init; } = 0.01m;
    public decimal MinSize { get; set; } = 1;
    public decimal? MinSizeMoney { get; set; }
    public decimal MaxSize { get; set; } = 1000000m;
    public decimal? MaxSizeMoney { get; set; }
    public decimal SizeIncrement { get; set; } = 1;
}