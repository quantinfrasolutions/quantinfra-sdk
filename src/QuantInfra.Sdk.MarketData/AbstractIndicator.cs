using Microsoft.Extensions.Logging;

namespace QuantInfra.Sdk.MarketData;

public struct NestedIndicator
{
    public NestedIndicator(AbstractIndicator indicator, int lookback)
    {
        Indicator = indicator;
        Lookback = lookback;
    }

    public AbstractIndicator Indicator { get; }
    public int Lookback { get; }
}

public abstract class AbstractIndicator
{
    private Dictionary<string, NestedIndicator> _requiredIndicators { get; } = new Dictionary<string, NestedIndicator>();

    protected ILogger Logger { get; private set; }


    /// <summary>
    /// This is an internal field used by BarsStorage that defines the number of bars in the stack
    /// at which the calculation of the indicator begins (but the value persisted at the bars between
    /// SkipBars and ReadyAt will be invalid and is used only for warming the indicator up).
    /// SkipBars == 0 means the calculation starts from the first bar.
    /// </summary>
    public int SkipBars { get; set; }
    /// <summary>
    /// This is an internal field used by BarsStorage that defines the number of bars in the stack
    /// at which the indicator shows the first valid value.
    /// ReadyAt == 1 means the correct value is available at the first bar.
    /// </summary>
    public int ReadyAt { get; set; }

    /// <summary>
    /// Id must be a unique reference of an indicator, combining the indicator name and parameters
    /// separated by colon, e.g. "SimpleMovingAverage:9:Close"
    /// </summary>
    public string Id
    {
        get => id;
        protected init
        {
            id = value;
            idHash = value.GetHashCode();
        }
    }

    private string id;
    protected int idHash;

    /// <summary>
    /// This is the number of bars in history after which an indicator starts returning a correct value.
    /// Use this for recursive indicators like EMA, that address only the previous bar in history (Bars[1])
    /// to calculate the current value, but before iterating over some number of bars are not calculated correctly.
    /// For the EMA example use WarmupBars = period - 1. For an indicator available immediately, use WarmupBars = 0.
    /// The strategy will start calculation only when the indicator gets its first valid value,
    /// i.e. when the number of bars in the storage equals WarmupBars.
    /// Also, this defines how many bars are requested from the history upon launch.
    /// </summary>
    public int WarmupBars { get; protected init; }
    public bool IsSeparateWindow { get; protected init; }
        

    public IEnumerable<NestedIndicator> GetNestedIndicators() => _requiredIndicators.Values;

    /// <summary>
    /// Call this in the constructor of your indicator for all first-level indicators you depend on (i.e. the indicators that will be accessed directly in the Calculate() method)
    /// </summary>
    /// <param name="indicator"></param>
    /// <param name="lookback">Number of historical bars required. For instance:
    ///     - If you use _close.Take(5), call RegisterIndicator(_close, 4);
    ///     - If you use _close.GetValue(bars[5]), call RegisterIndicator(_close, 5);
    /// </param>
    public void RegisterIndicator(AbstractIndicator indicator, int lookback = 0)
    {
        _requiredIndicators.TryAdd(indicator.Id, new NestedIndicator(indicator, lookback));
    }
        
    internal void RegisterLogger(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// This is the main calculation function
    /// </summary>
    /// <param name="bars">Stack of bars, from the newest (0) to the oldest</param>
    /// <param name="price"></param>
    protected abstract double? Calculate(IBarStorage bars, double? price = null);

    public void CalculateIndicator(IBarStorage bars, double? price = null)
    {
        var value = Calculate(bars, price);
        if (value != null)
        {
            bars.CurrentBar.AppendIndicator(idHash, value);
        }
    }

    public double? GetValue(IBar bar)
    {
        // return bar[Id];
        return bar[idHash];
    }

    public virtual string GetLogInformation(IBar bar) => $"{{'{Id}':{GetValue(bar)}}}";

    public IndicatorDescription GetIndicatorInfo() => new IndicatorDescription(
        Id,
        SkipBars,
        ReadyAt,
        WarmupBars,
        IsSeparateWindow,
        _requiredIndicators.Keys.ToList()
    );
}


public abstract class AbstractIndicator<T> : AbstractIndicator
    where T:struct
{        
    protected abstract (double?, T?) CalculateAndPersistData(IBarStorage bars, double? price = null);

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        (double? value, object? data) = CalculateAndPersistData(bars, price);
        if (data != null)
        {
            bars.CurrentBar.AppendIndicatorData(idHash, data);
        }
        //var value = Calculate(bars, price);
        //if (value != null)
        //{
        //    bars.CurrentBar.AppendIndicator(Id, value);
        //}
        return value;
    }

    public T? GetData(IBar bar) => (T?)bar.GetIndicatorData(idHash);
}