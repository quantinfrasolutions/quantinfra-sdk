using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace QuantInfra.Sdk.StaticData;

public class TradingSession
{
	public TradingSession() { }
	
	public TradingSession(int tradingSessionId, int exchangeId, Exchange exchange, ICollection<TradingSessionInterval> days, bool is24X7, bool isRth)
	{
		TradingSessionId = tradingSessionId;
		ExchangeId = exchangeId;
		Exchange = exchange;
		Days = days;
		Is24X7 = is24X7;
		IsRth = isRth;
	}

	public int TradingSessionId { get; init; }
	public string Name { get; init; } = string.Empty;
	public int ExchangeId { get; init; }
	public Exchange Exchange { get; init; }
	public ICollection<TradingSessionInterval> Days { get; init; }			
	public bool Is24X7 { get; init; }
	public bool IsRth { get; init; }

	public static ICollection<TradingSessionInterval> CreateSameScheduleSession(IEnumerable<IsoDayOfWeek> days, LocalTime start, LocalTime end) =>
		days
			.Select(d => new TradingSessionInterval { StartDay = d, Start = start, EndDay = d, End = end })
			.ToList();

	public static readonly ICollection<TradingSessionInterval> DefaultTradingSchedule = Enumerable.Range(1, 7)
		.Select(i => new TradingSessionInterval
		{
			StartDay = (IsoDayOfWeek)i, Start = LocalTime.Midnight, EndDay = (IsoDayOfWeek)i,
			End = LocalTime.Midnight
		})
		.ToList();

	public static TradingSession DefaultTradingSession =
		new TradingSession(0, 0, new(), DefaultTradingSchedule, true, true);
}