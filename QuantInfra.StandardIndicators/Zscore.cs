// using System;
// using System.Linq;
// using QuantInfra.Sdk.MarketData;
//
// namespace QuantInfra.StandardIndicators;
//
// /// <summary>
// /// <para> Description </para>   
// /// Z-score is a statistical measurement that describes a value's relationship to the mean of a group of values.
// /// Z-score is measured in terms of standard deviations from the mean.
// /// If a Z-score is 0, it indicates that the data point's score is identical to the mean score.
// /// </summary>
// public class Zscore : AbstractIndicator
// {
//     int _period;
//     AbstractIndicator _open, _close;
//     MathNet.Numerics.Distributions.Normal _cdf;
//
//     public Zscore(AbstractIndicator open = null,AbstractIndicator close = null,
//         int period = 9)
//     {
//         _open = open ?? new Open();
//         _period = period;
//         _close = close ?? new Close();
//         _cdf =  new MathNet.Numerics.Distributions.Normal();
//
//         RegisterIndicator(_open, _period - 1);
//         RegisterIndicator(_close, _period - 1);
//
//         Id = $"Zscore:{period}({_open.Id},{_close.Id})";
//         IsSeparateWindow = true;
//     }
//
//     protected override double? Calculate(IBarStorage bars, double? price = null)
//     {
//         var returns  = bars.Take(_period).Select(b=> _open.GetValue(b) - _close.GetValue(b));
//         double? avgRet = returns.Average();
//         double? retSum = returns.Sum(r => (r - avgRet) * (r - avgRet));
//         double stDev = Math.Sqrt((double)(retSum / _period));
//         return _cdf.CumulativeDistribution((double)(avgRet / stDev / Math.Sqrt(_period)));
//     }
// }