namespace QuantInfra.Sdk.Strategies;

public enum StopOrdersExecution
{
    /// <summary>
    /// Stop price of the order
    /// </summary>
    StopPx,
    /// <summary>
    /// Close price of the bar
    /// </summary>
    BarClose,
    /// <summary>
    /// The price by which the order was triggered (High/Low when checking inside the bar)
    /// </summary>
    TriggerPrice
}