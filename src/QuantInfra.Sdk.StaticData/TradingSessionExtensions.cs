using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace QuantInfra.Sdk.StaticData;

public static class TradingSessionExtensions
{
    public static IReadOnlyList<TradingScheduleInterval> GetSchedule(this ICollection<TradingSession> tradingSessions)
    {
        if (tradingSessions.Count == 1)
        {
            var ts = tradingSessions.Single();
            if (ts.Is24X7)
                return new List<TradingScheduleInterval>
                {
                    new(new(IsoDayOfWeek.Sunday, LocalTime.Midnight), new(IsoDayOfWeek.Saturday, LocalTime.Midnight), ts, 0)
                };
        }

        var breakpoints = tradingSessions.SelectMany(ts =>
            ts.Days.SelectMany(d => 
                new[] { new DayTime(d.StartDay, d.Start), new DayTime(d.EndDay, d.End) }
            ))
            .Distinct()
            .OrderBy(d => d)
            .ToArray();
        
        var intervals = new List<TradingScheduleInterval>(breakpoints.Length);

        for (var i = 0; i < breakpoints.Length - 1; i++)
        {
            var start = breakpoints[i];
            var end = breakpoints[i + 1];
            var ts = tradingSessions.SingleOrDefault(ts => ts.Days.Any(d => d.GetStartDayTime() == start && d.GetEndDayTime() == end));
            intervals.Add(new TradingScheduleInterval(start, end, ts, i));
        }

        if (intervals.Count > 1)
        {
            var first = intervals.First();
            var last = intervals.Last();
            var ts = tradingSessions.SingleOrDefault(ts => ts.Days.Any(d => d.GetStartDayTime() == last.End && d.GetEndDayTime() == first.Start));
            if (last.End != first.Start) intervals.Add(new TradingScheduleInterval(last.End, first.Start, ts, intervals.Count));
        }
        
        return intervals;
    }

    public static double GetTradingMinutesPerWeek(this IReadOnlyCollection<TradingScheduleInterval> schedule)
    {
        if (schedule.Count == 0) return 0;
        return schedule.Where(i => i.TradingSession is not null).Select(i => i.End - i.Start)
            .Sum(d => d.TotalMinutes);
    }
    
    public static TradingScheduleInterval? GetTradingSession(this IEnumerable<TradingScheduleInterval> schedule, DayTime dt) =>
        schedule.SingleOrDefault(i => i.Start <= dt && i.End > dt);
    
    public static Instant GetInstant(LocalDateTime now, DayTime dt, DateTimeZone tz)
    {
        var dtNow = new DayTime(now.DayOfWeek, now.TimeOfDay);
        var diff = dt - dtNow;
        var instant = now.InZoneStrictly(tz).ToInstant();
        return dtNow > dt ? instant.Minus(diff) : dtNow < dt ? instant.Plus(diff) : instant;
    }
}


public class TradingScheduleInterval
{
    public TradingScheduleInterval(DayTime start, DayTime end, TradingSession? tradingSession, int order)
    {
        Start = start;
        End = end;
        TradingSession = tradingSession;
        Order = order;
    }

    public DayTime Start { get; init; }
    public DayTime End { get; init; }
    public TradingSession? TradingSession { get; init; }
    public int Order { get; init; }
}