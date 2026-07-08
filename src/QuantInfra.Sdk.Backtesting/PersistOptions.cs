namespace QuantInfra.Sdk.Backtesting;

public class PersistOptions
{
    /// <summary>
    /// Persist strategy configs
    /// </summary>
    public bool SaveStrategies { get; init; } = false;
    
    /// <summary>
    /// Persist trades
    /// </summary>
    public bool SaveTrades { get; init; } = true;
    
    /// <summary>
    /// Used to pre-initialize the trades buffer to increase test speed
    /// </summary>
    public int ExpectedNumberOfTradesPerDay { get; init; } = 5;
    
    /// <summary>
    /// Persist deals (closed positions with attributed realizedPnL)
    /// </summary>
    public bool SavePositions { get; init; } = true;
    
    /// <summary>
    /// Persist daily returns and yield history 
    /// </summary>
    public bool SaveDailyReturns { get; init; } = true;
    
    /// <summary>
    /// Skip days where return is zero
    /// </summary>
    public bool DoNotSaveZeroDailyReturns { get; init; } = false;
    
    /// <summary>
    /// Persist values of positions and balances at each end of day
    /// </summary>
    public bool SaveEndOfDayValues { get; init; } = true;
    
    /// <summary>
    /// Used to pre-initialize the positions buffer to increase test speed
    /// </summary>
    public int ExpectedNumberOfOpenPositionsAtEndOfDay { get; init; } = 5;
    
    /// <summary>
    /// Persist metrics
    /// </summary>
    public bool SaveMetrics { get; init; } = true;
}