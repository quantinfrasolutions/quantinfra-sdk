using System.Reflection;
using NodaTime;
using QuantInfra.Sdk.Trading.Positions;

namespace QuantInfra.Sdk.Trading.Tests;

public class PnLCalculatorTests
{
    [Test]
    [TestCase(1, 0)]
    [TestCase(0.1, 1)]
    [TestCase(0.01, 2)]
    [TestCase(0.03, 2)]
    [TestCase(0.05, 2)]
    [TestCase(0.08, 2)]
    [TestCase(5, 0)]
    public void TestPnLCalculatorWaPrecision(decimal tickSize, int waPrecision)
    {
        var options = new PnLCalculatorOptions(PnLCalculatorType.Default, tickSize, tickSize, 2);
        var calculator = options.GetCalculator();
        var fi = calculator.GetType().GetField("_waPrecision", BindingFlags.NonPublic | BindingFlags.Instance);
        var val = (int)fi.GetValue(calculator); 
        Assert.That(val, Is.EqualTo(waPrecision));
    }

    [Test]
    [TestCase(PnLCalculatorType.Default, 0.1, 0, 2, 10000, 3, 30000)]
    [TestCase(PnLCalculatorType.Default, 0.01, 0, 2, 10000, -3, -30000)]
    [TestCase(PnLCalculatorType.Futures, 0.01, 3, 2, 2.56, 2, 1536)]
    [TestCase(PnLCalculatorType.Futures, 1, 5, 2, 10, -6, -300)]
    [TestCase(PnLCalculatorType.InverseFutures, 0.1, 10, 8, 50000, 2, 0.004)] // Binance BTCUSD CoinM
    [TestCase(PnLCalculatorType.InverseFutures, 0.1, 10, 8, 50000, -2, -0.004)] // Binance BTCUSD CoinM
    public void TestValueAndPrice(PnLCalculatorType type, decimal tickSize, decimal tickValue, int settlCcyPrecision,
        decimal price, decimal signedVolume, decimal expectedValue)
    {
        var options = new PnLCalculatorOptions(type, tickSize, tickValue, settlCcyPrecision);
        var calculator = options.GetCalculator();

        var value = calculator.GetValueInSettlementCcy(price, signedVolume);
        Assert.That(value, Is.EqualTo(expectedValue));
        
        var calcPrice = calculator.GetPrice(signedVolume, value);
        Assert.That(calcPrice, Is.EqualTo(price));

        var calcVolume = calculator.GetVolume(value, price);
        Assert.That(calcVolume, Is.EqualTo(signedVolume));
    }

    [Test]
    [TestCase(PnLCalculatorType.Default, 0.1, 0, 2, 10000, 11000, 3, 3000)]
    [TestCase(PnLCalculatorType.Default, 0.1, 0, 2, 10000, 9500, 3, -1500)]
    [TestCase(PnLCalculatorType.Default, 0.1, 0, 2, 10000, 11000, -3, -3000)]
    [TestCase(PnLCalculatorType.Default, 0.1, 0, 2, 10000, 9500, -3, 1500)]
    [TestCase(PnLCalculatorType.Futures, 0.01, 3, 2, 2.56, 2.6, 2, 24)]
    [TestCase(PnLCalculatorType.Futures, 0.01, 3, 2, 2.56, 2.52, 2, -24)]
    [TestCase(PnLCalculatorType.Futures, 0.01, 3, 2, 2.56, 2.6, -2, -24)]
    [TestCase(PnLCalculatorType.Futures, 0.01, 3, 2, 2.56, 2.52, -2, 24)]
    [TestCase(PnLCalculatorType.InverseFutures, 0.1, 10, 4, 50000, 68762, 6, 0.0032)]
    [TestCase(PnLCalculatorType.InverseFutures, 0.1, 10, 4, 50000, 42000, 6, -0.0022)]
    [TestCase(PnLCalculatorType.InverseFutures, 0.1, 10, 4, 50000, 68762, -6, -0.0032)]
    [TestCase(PnLCalculatorType.InverseFutures, 0.1, 10, 4, 50000, 42000, -6, 0.0022)]
    [TestCase(PnLCalculatorType.InverseFutures, 0.1, 10, 8, 93636.8, 93448.9, -234.09, 0.00050267)]
    public void TestPnL(PnLCalculatorType type, decimal tickSize, decimal tickValue, int settlCcyPrecision,
        decimal openPrice, decimal closePrice, decimal signedVolume, decimal expectedPnL)
    {
        var options = new PnLCalculatorOptions(type, tickSize, tickValue, settlCcyPrecision);
        var calculator = options.GetCalculator();

        var pnl = calculator.GetPnL(openPrice, Math.Abs(signedVolume), closePrice, signedVolume.FromSign());
        Assert.That(pnl, Is.EqualTo(expectedPnL));

        var position = Position.OpenPosition(10000, 10000, 10000, signedVolume, openPrice,
            calculator.GetValueInSettlementCcy(openPrice, Math.Abs(signedVolume)), 1, settlCcyPrecision,
            Instant.FromUtc(2026, 6, 8, 0, 0),
            false, null, null, 0, null);

        var (act, hist) = position.ReducePosition(10001, Math.Abs(position.Volume),
            closePrice,
            calculator.GetValueInSettlementCcy(closePrice, Math.Abs(signedVolume)),
            1, settlCcyPrecision,
            Instant.FromUtc(2026, 6, 8, 0, 1),
            0, settlCcyPrecision, null, calculator
        );
        
        Assert.That(act, Is.Null);
        Assert.That(hist, Is.Not.Null);
        Assert.That(hist.RealizedPnL, Is.EqualTo(expectedPnL).Within(Math.Pow(10, -settlCcyPrecision)));

        (act, hist) = position.MarkToMarket(Instant.FromUtc(2026, 6, 8, 0, 1),
            closePrice, 
            calculator.GetValueInSettlementCcy(closePrice, signedVolume),
            calculator.GetValueInSettlementCcy(closePrice, signedVolume),
            calculator);
        
        Assert.That(hist, Is.Not.Null);
        Assert.That(hist.FloatingPnL, Is.EqualTo(expectedPnL).Within(Math.Pow(10, -settlCcyPrecision)));
        Assert.That(hist.TotalFloatingPnL, Is.EqualTo(hist.FloatingPnL));
    }

    [Test]
    [TestCase(PnLCalculatorType.Default, 0.1, 0, 2, 10000, 5, 11000, 5, 10500)]
    [TestCase(PnLCalculatorType.Default, 0.1, 0, 2, 10000, -5, 11000, -5, 10500)]
    public void TestWeightedAverage(PnLCalculatorType type, decimal tickSize, decimal tickValue, int settlCcyPrecision,
        decimal p1, decimal v1, decimal p2, decimal v2, decimal expected)
    {
        var options = new PnLCalculatorOptions(type, tickSize, tickValue, settlCcyPrecision);
        var calculator = options.GetCalculator();
        
        var wa = calculator.GetWA(p1, v1, p2, v2);
        Assert.That(wa, Is.EqualTo(expected));
    }
}