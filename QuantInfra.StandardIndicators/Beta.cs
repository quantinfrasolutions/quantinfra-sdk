using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators;

/// <summary>
/// <para> Description </para>   
/// Beta indicator is used to measure volatility or risk of a stock 
/// relative to the overall risk (volatility) of the stock market.
/// TODO:https://www.marketvolume.com/technicalanalysis/beta.asp implement this.
/// </summary>
public class Beta : AbstractIndicator
{
    int _period;
    ROC _benchmarkRoc, _roc;

    public Beta(AbstractIndicator benchmarkClose = null, AbstractIndicator close = null,
        int period = 14)
    {
        _period = period;
        benchmarkClose = benchmarkClose ?? new Close();
        close = close ?? new Close();
        _roc = new ROC(close, 1);
        _benchmarkRoc = new ROC(benchmarkClose, 1);
        RegisterIndicator(_benchmarkRoc, _period - 1);
        RegisterIndicator(_roc, _period - 1);

        Id = $"Beta:{period}({benchmarkClose.Id},{close.Id})";
        IsSeparateWindow = true;
    }


    protected override double? Calculate(IBarStorage bars, double? price = null)
    {
        double? rocCum = 0;
        double? rocCumSqrt = 0;
        double? benchRocCum = 0;
        double? benchRocCumSqrt = 0;
        for (int i = 0; i < _period; i++)
        {
            rocCum += _roc.GetValue(bars[i]);
            benchRocCum += _benchmarkRoc.GetValue(bars[i]);
            benchRocCumSqrt += Math.Pow((double)_benchmarkRoc.GetValue(bars[i]),2);
            rocCumSqrt += _benchmarkRoc.GetValue(bars[i])* _roc.GetValue(bars[i]);
        }
        return (rocCumSqrt * _period - rocCum*benchRocCum)/(benchRocCumSqrt*_period - Math.Pow((double)benchRocCum, 2));
    }
}