namespace QuantInfra.Sdk.StaticData;

public class Stream
{
    public int StreamId { get; init; }
    public string? Ticker { get; init; }
    public int DatafeedId { get; init; }
    public Contract? Contract { get; init; }
    public ConstantStreamValue? ConstantStreamValue { get; init; }
}