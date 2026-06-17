using System.Collections.Generic;

namespace QuantInfra.Sdk.StaticData;

public class Currency
{		
	public int CurrencyId { get; init; }
	public int Decimals { get; init; }
	public Asset Asset { get; init; }
	public ICollection<CurrencyOverride> BrokerOverrides { get; init; }
}