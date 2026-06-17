using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    public class EZeroCoef : AbstractIndicator
    {
        AbstractIndicator _underlyingIndicator;
        double? alpha;

        public EZeroCoef(AbstractIndicator ind, int period = 9, int power = 2)
        {
            double beta = 0.45 * (period - 1) / (0.45 * (period - 1) + 2);
            alpha = Math.Pow((double)beta, power);
            _underlyingIndicator = ind ?? new Close();
            RegisterIndicator(_underlyingIndicator, period);

            Id = $"EZeroCoef:{period}({_underlyingIndicator.Id})";
        }

        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            var currentValue = _underlyingIndicator.GetValue(bars.CurrentBar);
            if (!currentValue.HasValue) return null;

            var previousValue = bars.Count > 1 ? GetValue(bars[1]) : null;
            if (!previousValue.HasValue) return currentValue; 

            return (1 - alpha) * _underlyingIndicator.GetValue(bars.CurrentBar) + alpha * GetValue(bars[1]);
        }
    }

    public class EOneCoef : AbstractIndicator
    {
        AbstractIndicator _underlyingIndicator;
        double? beta;
        EZeroCoef _e0;

        public EOneCoef(AbstractIndicator ind, int period = 9, int power = 2)
        {
            beta = 0.45 * (period - 1) / (0.45 * (period - 1) + 2);
            _e0 = new EZeroCoef(ind, period, power);
            _underlyingIndicator = ind ?? new Close();
            RegisterIndicator(_underlyingIndicator);
            RegisterIndicator(_e0);

            Id = $"EOneCoef:{period}({_underlyingIndicator.Id})";
        }

        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            var currentValue = _underlyingIndicator.GetValue(bars.CurrentBar);
            if (!currentValue.HasValue) return null;
            var previousValue = bars.Count > 1 ? GetValue(bars[1]) : null;
            if (!previousValue.HasValue) return currentValue;
            return (_underlyingIndicator.GetValue(bars.CurrentBar) - _e0.GetValue(bars[0])) * (1 - beta) + beta * GetValue(bars[1]);
        }
    }

    public class ETwoCoef : AbstractIndicator
    {
        double? phaseRatio,alpha;
        EZeroCoef _e0;
        EOneCoef _e1;

        public ETwoCoef(AbstractIndicator ind, int period = 9, int phase = 50, int power = 2)
        {
            ind = ind ?? new Close();
            double beta = 0.45 * (period - 1) / (0.45 * (period - 1) + 2);
            alpha = Math.Pow((double)beta, power);
            phaseRatio = phase < -100 ? 0.5 : phase > 100 ? 2.5 : phase / 100 + 1.5;
            _e0 = new EZeroCoef(ind, period, power);
            _e1 = new EOneCoef(ind, period, power); 

            RegisterIndicator(_e0);
            RegisterIndicator(_e1);

            Id = $"ETwoCoef:{period}({ind.Id})";
        }

        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            var previousValue = bars.Count > 1 ? GetValue(bars[1]) : null;
            if (!previousValue.HasValue) return 0;
            return (_e0.GetValue(bars[0]) + phaseRatio * _e1.GetValue(bars[0]) - GetValue(bars[1]))
                * Math.Pow((double)(1 - alpha), 2) + Math.Pow((double)alpha, 2) * GetValue(bars[1]);
        }
    }

    /// <summary>
    /// <para> Description </para> 
    /// The JMA Indicator is a unique moving average that utilizes a complex mathematical formula
    /// to smooth out price data and reduce noise in the market. It is designed to be more responsive
    /// to price changes than traditional moving averages
    /// </summary>
    public class JMA : AbstractIndicator
    {
        AbstractIndicator _underlyingIndicator;
        ETwoCoef _e2;
        
        public JMA(AbstractIndicator ind, int period = 9, int phase = 50,  int power = 2)
        {
            _e2 = new ETwoCoef(ind, period, phase, power);
           
            _underlyingIndicator = ind;
            RegisterIndicator(_e2);
            RegisterIndicator(_underlyingIndicator);

            Id = $"JMA:{period}:{phase}:{power}({_underlyingIndicator.Id})";
        }

        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            if ( GetValue(bars[1]) == null) return _underlyingIndicator.GetValue(bars.CurrentBar);
            return _e2.GetValue(bars[0]) + GetValue(bars[1]);
        }
    }
}

