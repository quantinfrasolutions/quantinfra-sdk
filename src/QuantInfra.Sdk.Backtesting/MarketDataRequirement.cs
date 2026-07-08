using System.Collections.Generic;

namespace QuantInfra.Sdk.Backtesting;

public record class MarketDataRequirement(int AccountCurrencyId, IReadOnlyCollection<int> ContractIds, IReadOnlyCollection<int> StreamIds);