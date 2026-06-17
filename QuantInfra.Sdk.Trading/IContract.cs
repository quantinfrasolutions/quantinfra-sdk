using System;
namespace Common.Trading
{
	public interface IContract
	{
        decimal NormalizePrice(decimal price);
        decimal NormalizePrice(double price);
    }
}

