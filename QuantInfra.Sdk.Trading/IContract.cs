namespace QuantInfra.Sdk.Trading
{
	public interface IContract
	{
        decimal NormalizePrice(decimal price);
        decimal NormalizePrice(double price);
    }
}

