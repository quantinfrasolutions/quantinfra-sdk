using System.Collections.Generic;
using NodaTime;

namespace QuantInfra.Sdk.StaticData;

public class Exchange
{        
	public int ExchangeId { get; init; }     
	public string Name { get; init; } = string.Empty;
	public string TimezoneName { get; init; } = "UTC";
	public DateTimeZone Timezone => DateTimeZoneProviders.Tzdb[TimezoneName];
	public ICollection<TradingSession> TradingSessions { get; init; }
}