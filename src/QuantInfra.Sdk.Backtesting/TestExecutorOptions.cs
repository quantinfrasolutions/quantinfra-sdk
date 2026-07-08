using Microsoft.Extensions.Logging;
using NodaTime;
using QuantInfra.Sdk.Strategies;

namespace QuantInfra.Sdk.Backtesting;

/// <summary>
/// The base class for defining parameters for testing actions
/// </summary>
public record class TestExecutorOptions
{
    public TestExecutorOptions() { }

    public TestExecutorOptions(Instant startDt, Instant endDt)
    {
        StartDt = startDt;
        EndDt = endDt;
    }
    
    
    /// <summary>
    /// Test start timestamp
    /// </summary>
    public Instant StartDt { get; init; }
    
    /// <summary>
    /// Test end timestamp
    /// </summary>
    public Instant EndDt { get; init; }
    
    /// <summary>
    /// Log level for transactional logging
    /// </summary>
    public LogLevel LogLevel { get; init; } = LogLevel.None;
    
    /// <summary>
    /// The base for calculating returns, in account currency
    /// </summary>
    public decimal Investment { get; init; } = 100000m;
    
    /// <summary>
    /// Timeframe of candles that will be used for the test. Reducing the timeframe increases precision, but increases the test time.
    /// </summary>
    public Period CandlesTimeframe { get; init; } = Period.FromMinutes(1);
    
    /// <summary>
    /// Offset from UTC midnight when end of day is run
    /// </summary>
    public Duration MtmUtcOffset { get; init; } = Duration.Zero;
    
    /// <summary>
    /// When bar storages are initialized, several attempts are made to retrieve the required number
    /// of basic aggregation units from the storage, each stepping to the beginning of the previous month.
    /// This approach tries to make sure there is enough bars to initialize the storages, not knowing how many units are
    /// available in the storage. 
    /// </summary>
    public int RequestBarAttempts { get; init; } = 1;
    
    /// <summary>
    /// Defines if an exception should be thrown when a strategy tries to place an order with zero volume. Recommended setup is
    /// true for backtesting (to catch the orders that cannot be placed) and false for live trading.
    /// </summary>
    public bool ThrowOnZeroVolumeOrders { get; init; }

    /// <summary>
    /// Defines the contract size step for virtual accounts. Fraction 100 means the volume step is 0.01.
    /// </summary>
    public int VirtualAccountSizeStepFraction { get; init; } = 100;
    // TODO: add option to use Contract.NormalizeVolume on virtual accounts
    
    /// <summary>
    /// If false, only Open/Close price will be used to trigger pending orders
    /// </summary>
    public bool CheckPendingOrdersExecutionUsingHighLow { get; init; } = false;
    
    /// <summary>
    /// Recommended use is true
    /// </summary>
    public bool CheckOrdersAtBarOpen { get; init; } = true;
    
    /// <summary>
    /// Recommended use is false
    /// </summary>
    public bool CheckOrdersAtBarClose { get; init; } = false;
    
    /// <summary>
    /// Recommended: true, indicates if only market orders placed at the end of the bar should be checked using the close price of this bar
    /// (when CheckOrdersAtBarClose == true)
    /// </summary>
    public bool LimitCloseCheckToMarketOrdersOnly { get; init; } = true;
    
    /// <summary>
    /// Defines the price by which stop orders are executed by Virtual Executor
    /// </summary>
    /// <remarks>
    /// Recommended usage scenarios are:
    /// * For backtesting using minute bars — BarClose
    /// * For backtesting using aggregated bars — StopPx
    /// </remarks>
    public StopOrdersExecution StopOrdersExecution { get; init; } = StopOrdersExecution.BarClose;
    
    /// <summary>
    /// When orders are executed at the open of the bar, their execution time will be OpenDt + offset
    /// </summary>
    public Duration OpenExecutionOffset { get; init; } = Duration.FromSeconds(1);
    /// <summary>
    /// When pending orders are executed inside the bar, their execution time will be OpenDt + offset
    /// </summary>
    public Duration HighLowExecutionOffset { get; init; } = Duration.FromSeconds(44);
}