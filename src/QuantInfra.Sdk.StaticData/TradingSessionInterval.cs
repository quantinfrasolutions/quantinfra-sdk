using System;
using NodaTime;

namespace QuantInfra.Sdk.StaticData;

public class TradingSessionInterval
{		
	public int IntervalId { get; init; }
	public int TradingSessionId { get; init; }
	public IsoDayOfWeek StartDay { get; init; }
	public LocalTime Start { get; init; }
	public IsoDayOfWeek EndDay { get; init; }
	public LocalTime End { get; init; }
	public DayTime GetStartDayTime() => new(StartDay, Start);
	public DayTime GetEndDayTime() => new(EndDay, End);
}

public readonly struct DayTime : IComparable<DayTime>, IEquatable<DayTime>
{
	public DayTime(IsoDayOfWeek day, LocalTime time)
	{
		Day = day;
		Time = time;
	}

	public IsoDayOfWeek Day { get; }
	public LocalTime Time { get; }
	
	
	private int GetMinutes() => (Day == IsoDayOfWeek.Sunday ? 0 : (int)Day) * 24 * 60 + Time.Hour * 60 + Time.Minute;

	public int CompareTo(DayTime other) => GetMinutes().CompareTo(other.GetMinutes());
	
	public static Duration operator - (DayTime end, DayTime start) => Duration.FromMinutes(Math.Abs(end.GetMinutes() - start.GetMinutes()));
	public static bool operator < (DayTime first, DayTime second) => first.GetMinutes() < second.GetMinutes();
	public static bool operator > (DayTime first, DayTime second) => first.GetMinutes() > second.GetMinutes();
	public static bool operator <= (DayTime first, DayTime second) => first.GetMinutes() <= second.GetMinutes();
	public static bool operator >= (DayTime first, DayTime second) => first.GetMinutes() >= second.GetMinutes();
	public static bool operator ==(DayTime first, DayTime second) => first.GetMinutes() == second.GetMinutes();
	public static bool operator !=(DayTime first, DayTime second) => first.GetMinutes() != second.GetMinutes();

	public bool Equals(DayTime other)
	{
		return Day == other.Day && Time.Equals(other.Time);
	}

	public override bool Equals(object? obj)
	{
		return obj is DayTime other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine((int)Day, Time);
	}
}