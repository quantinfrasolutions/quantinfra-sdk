namespace QuantInfra.Sdk.StaticData.Synthetics;

public enum SyntheticContractType
{
    /// <summary>
    /// Multiplies the current prices of contracts in power of weights.
    /// Can be used for FX convertion, e.g. -1 EUR/USD, +1 EUR/CHF gives a long spread USD/CHF
    /// </summary>
    Multiplicative,
    /// <summary>
    /// Adds the difference between current contract prices and prices at the beginning of the period, multiplied by contracts' weights
    /// to the price of the synthetic contract as of the beginning of the period.
    /// Can be used to trade index-like synthetic contracts that do not change the price sharply between periods.
    /// </summary>
    Index,
    /// <summary>
    /// Sums the product of current contract prices and their weights.
    /// Can be used to trade rolling contracts, e.g. first futures in the series.
    /// </summary>
    Rolling
}