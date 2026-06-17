namespace QuantInfra.Sdk.StaticData;

public class Broker
{
    public int BrokerId { get; init; }
    public string Name { get; init; } = string.Empty;
    public BrokerType BrokerType { get; init; }
}