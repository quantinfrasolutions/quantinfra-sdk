namespace QuantInfra.Sdk.Strategies;

public abstract class AbstractHostedStrategy<TParams> : AbstractHostedStrategy
    where TParams : class, new()
{
    protected AbstractHostedStrategy(StrategyConfig sc) : base(sc)
    {
        Params = sc.DeserializeParams<TParams>() ?? new();
    }

    public TParams Params { get; }
}