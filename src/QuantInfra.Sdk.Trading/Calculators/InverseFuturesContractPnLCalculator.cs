using System;

namespace QuantInfra.Sdk.Trading.Calculators
{
    public sealed class InverseFuturesContractPnLCalculator : IPnLCalculator
    {
        private readonly int _waPrecision, _settlCurrencyPrecision;
        private readonly decimal _tickSize, _tickValue;

        public InverseFuturesContractPnLCalculator(
            decimal tickSize = 0.0001m,
            decimal tickValue = 0.0001m,
            int waPrecision = 10,
            int settlCurrencyPrecision = 2
        )
        {
            _tickSize = tickSize;
            _tickValue = tickValue;
            _waPrecision = waPrecision;
            _settlCurrencyPrecision = settlCurrencyPrecision;            
        }
        

        public decimal GetPnL(decimal openPrice, decimal volume, decimal currentPrice, Side side, int? precision = null) =>
            currentPrice == 0 || openPrice == 0 
                ? 0m
                : Math.Round(
                    (volume * _tickValue / (openPrice * _tickSize) - volume * _tickValue / (currentPrice * _tickSize)) * side.GetSign(), 
                    precision ?? _settlCurrencyPrecision,
                    (MidpointRounding)2 // ToZero
                );

        public decimal GetPnL(decimal signedOpenValue, decimal signedCloseValue) => signedOpenValue - signedCloseValue;

        public decimal GetValueInSettlementCcy(decimal price, decimal volume, int? precision = null) =>
            price == 0 ? 0 : Math.Round(volume * _tickValue / (price * _tickSize), precision ?? _settlCurrencyPrecision);

        public decimal GetPrice(decimal signedVolume, decimal value) =>
            value == 0 ? 0 : signedVolume * _tickValue / (value * _tickSize);

        public decimal GetVolume(decimal value, decimal price) => value * price * _tickSize / _tickValue;
        
        public decimal GetWA(decimal price1, decimal volume1, decimal price2, decimal volume2, int? precision = null)
        {
            var price= GetPrice(volume1 + volume2,
                GetValueInSettlementCcy(price1, volume1) + GetValueInSettlementCcy(price2, volume2)
            );
            return Math.Round(price, precision ?? _waPrecision);
        }
    }
}
