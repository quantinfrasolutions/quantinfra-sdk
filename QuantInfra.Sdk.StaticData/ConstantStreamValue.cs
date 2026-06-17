namespace QuantInfra.Sdk.StaticData;

public class ConstantStreamValue
{
    public int StreamId { get; init; }
    public decimal Value { get; init; }
    // TODO
    public string CronExpression => "* * * ? * * *";
}