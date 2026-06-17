using System;

namespace Common.Trading
{
    public sealed class DefaultFuturesContractPLCalculator : IPnLCalculator
    {
        private readonly int _waPrecision;
        private readonly int _settlCurrencyPrecision;
        private readonly decimal _tickSize;
        private readonly decimal _priceQuotation;
        private readonly decimal _tickValue;

        public DefaultFuturesContractPLCalculator(
            decimal tickSize = 0.0001m,
            decimal tickValue = 0.0001m,
            decimal priceQuotation = 1m,
            int waPrecision = 10,
            int settlCurrencyPrecision = 2
        )
        {
            _tickSize = tickSize;
            _tickValue = tickValue;
            _priceQuotation = priceQuotation;
            _waPrecision = waPrecision;
            _settlCurrencyPrecision = settlCurrencyPrecision;            
        }
        

        public decimal GetPnL(decimal openPrice, decimal volume, decimal currentPrice, Side side, int? precision = null) =>
            Math.Round((currentPrice - openPrice) * volume / _tickSize * _tickValue * side.GetSign(), precision ?? _settlCurrencyPrecision);

        public decimal GetValueInSettlementCcy(decimal price, decimal volume, int? precision = null) =>
            Math.Round(price / _tickSize * _tickValue * volume, precision ?? _settlCurrencyPrecision);

        public decimal GetPrice(decimal signedVolume, decimal value) =>
            signedVolume == 0 ? 0 : (value * _tickSize) / _tickValue / Math.Abs(signedVolume);

        public decimal GetVolume(decimal value, decimal price) => price == 0
            ? 0
            : value / price * _tickSize / _tickValue;

        public decimal GetWA(decimal price1, decimal volume1, decimal price2, decimal volume2, int? precision = null) =>
            volume1 + volume2 == 0 ? 0 :
            Math.Round((price1 * volume1 + price2 * volume2) / (volume1 + volume2), precision ?? _waPrecision);
    }
}
