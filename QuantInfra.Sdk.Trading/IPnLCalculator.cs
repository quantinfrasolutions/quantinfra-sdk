
namespace Common.Trading
{
    public interface IPnLCalculator
    {
        decimal GetWA(decimal price1, decimal volume1, decimal price2, decimal volume2, int? precision = null);
        decimal GetPnL(decimal openPrice, decimal volume, decimal currentPrice, Side side, int? precision = null);
        decimal GetValueInSettlementCcy(decimal price, decimal volume, int? precision = null);
        decimal GetPrice(decimal signedVolume, decimal value);
        decimal GetVolume(decimal value, decimal price);
    }
}
