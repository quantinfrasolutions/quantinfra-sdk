using System;
using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para> 
    /// NormilizedRage is simply absoulte value of difference of 2 underlyings.
    /// </summary>
    public class NormilizedRage : AbstractIndicator
    {
        AbstractIndicator _underlyingIndicator;
        AbstractIndicator _underlyingIndicatorSubstruct;

        public NormilizedRage(AbstractIndicator ind, AbstractIndicator indSubstruct)
        {
            _underlyingIndicator = ind ?? new Close();
            _underlyingIndicatorSubstruct = indSubstruct ?? new Close();
            RegisterIndicator(_underlyingIndicator);
            RegisterIndicator(_underlyingIndicatorSubstruct);

            Id = $"NormilizedRage({_underlyingIndicator.Id},{_underlyingIndicatorSubstruct})";
            IsSeparateWindow = true;
        }

        protected override double? Calculate(IBarStorage bars, double? price = null)
            => Math.Abs(Convert.ToDouble(_underlyingIndicator.GetValue(bars[0]) - _underlyingIndicatorSubstruct.GetValue(bars[0])));
    }
}