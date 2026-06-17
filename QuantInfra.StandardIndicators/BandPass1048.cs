using QuantInfra.Sdk.MarketData;

namespace QuantInfra.StandardIndicators
{
    /// <summary>
    /// <para> Description </para>   
    /// Filter of high frequency noises. Calculated with 4 constant values.
    /// Constants are set in according with R&D.
    /// TODO: explore the full descritpion of constants.
    /// </summary>
    public class BandPass1048 : AbstractIndicator
    {
        AbstractIndicator _source;    

        const double _b0 = 0.202602147502353557850796050843200646341;
        const double _b2 = -0.202602147502353557850796050843200646341;
        const double _a1 = -1.528389936666702642042992010829038918018;
        const double _a2 = 0.594795704995292884298407898313598707318;

        public BandPass1048(AbstractIndicator source = null)
        {
            _source = source ?? new Close();
            
            RegisterIndicator(_source);

            Id = $"BandPass1048:{_source.Id}";
            IsSeparateWindow = true;
            WarmupBars = 2;
        }


        protected override double? Calculate(IBarStorage bars, double? price = null)
        {
            var currentValue = _source.GetValue(bars.CurrentBar);
            if (!currentValue.HasValue) return null;
            if (bars.Count < 2 || !GetValue(bars[1]).HasValue) return currentValue;
            return _b0 * currentValue + _b2 * _source.GetValue(bars[2]) - _a1 * GetValue(bars[1]) - _a2 * GetValue(bars[2]);
        }
    }
}