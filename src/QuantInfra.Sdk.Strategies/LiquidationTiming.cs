namespace QuantInfra.Sdk.Strategies;

public enum LiquidationTiming
{
    /// <summary>
    /// Marks account to market at every BAU (minute bar), liquidates based on equity.
    /// </summary>
    OnBarClose,
    /// <summary>
    /// Checks the risk limits when any position gets reduced.
    /// Depending on the account settings, will or will not include floating profit from still open positions.  
    /// </summary>
    OnPositionClose,
    /// <summary>
    /// Checks the risk limits at the end of day.
    /// Depending on the account settings, will or will not include floating profit from still open positions.
    /// </summary>
    OnEndOfDay
}