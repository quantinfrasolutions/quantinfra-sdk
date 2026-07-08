namespace QuantInfra.Sdk.MarketData;

public record class IndicatorDescription
{
	public string Id { get; init; }
	public int SkipBars { get; init; }
	public int ReadyAt { get; init; }
	public int WarmupBars { get; init; }
	public bool IsSeparateWindow { get; init; }
	public List<string> NestedIndicators { get; init; }

	public IndicatorDescription() { }

	public IndicatorDescription(string id, int skipBars, int readyAt, int warmupBars, bool isSeparateWindow, List<string> nestedIndicators)
	{
		Id = id;
		SkipBars = skipBars;
		ReadyAt = readyAt;
		WarmupBars = warmupBars;
		IsSeparateWindow = isSeparateWindow;
		NestedIndicators = nestedIndicators;
	}
}