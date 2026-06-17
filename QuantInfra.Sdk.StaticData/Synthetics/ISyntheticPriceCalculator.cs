using System.Collections.Generic;

namespace QuantInfra.Sdk.StaticData.Synthetics;

public interface ISyntheticPriceCalculator
{
    double CalculatePrice(SyntheticContractComposition composition, IReadOnlyDictionary<int, double> prices);
    double CalculateVolume(SyntheticContractComposition composition, Dictionary<int, double> volumes);
}