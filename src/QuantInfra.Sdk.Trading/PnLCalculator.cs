using System;
using QuantInfra.Sdk.Trading.Calculators;

namespace QuantInfra.Sdk.Trading;

public readonly record struct PnLCalculatorOptions(
    PnLCalculatorType Type,
    decimal TickSize,
    decimal TickValue,
    int SettlCcyPrecision
);

public static class PnLCalculator
{
    public static IPnLCalculator GetCalculator(this PnLCalculatorOptions options)
    {
        var waPrecision = (int)(-Math.Round(Math.Log10((double)options.TickSize), (MidpointRounding)3)); // MidpointRounding.ToNegativeInfinity
        return options.Type switch
        {
            PnLCalculatorType.Default => new DefaultPnLCalculator(waPrecision, settlCurrencyPrecision: options.SettlCcyPrecision),
            PnLCalculatorType.Futures => new FuturesPnLCalculator(options.TickSize, options.TickValue, settlCurrencyPrecision: options.SettlCcyPrecision, waPrecision: waPrecision),
            PnLCalculatorType.InverseFutures => new InverseFuturesContractPnLCalculator(options.TickSize, options.TickValue, settlCurrencyPrecision: options.SettlCcyPrecision, waPrecision: waPrecision),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}