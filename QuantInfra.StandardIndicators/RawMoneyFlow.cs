using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Row Money Flow is a part of Money Flow Index. The value is Typical price
/// multiplied by Volume.
/// </summary>
public class RawMoneyFlow : AbstractIndicator
{
    HLC3 _typicalPrice;

    public RawMoneyFlow(AbstractIndicator high = null, AbstractIndicator low = null,
        AbstractIndicator close = null)
    {
        high = high ?? new High();
        low = low ?? new Low();
        close = close ?? new Close();
        _typicalPrice = new HLC3(high,low,close);

        RegisterIndicator(_typicalPrice);

        Id = $"RowMoneyFlow({high.Id},{low.Id},{close.Id})";
        IsSeparateWindow = true;
    }

    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        return _typicalPrice.GetValue(bars[0]) * bars[0].Volume;
    }
}