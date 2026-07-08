using System;

namespace QuantInfra.Sdk.Accounting;

public record DailyReturn(decimal Return, Guid AccountId, Guid? StrategyId = null);