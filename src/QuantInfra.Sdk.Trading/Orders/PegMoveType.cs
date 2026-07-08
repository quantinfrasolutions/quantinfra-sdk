namespace QuantInfra.Sdk.Trading.Orders;

public enum PegMoveType
{
	/// <summary>
	/// Standard pegged order (moves to Close * (1 -/+ PegOffset)
	/// </summary>
	Floating = 0,
	/// <summary>
	/// Standard pegged order with fixed peg (moves to Close +/- PegOffset)
	/// </summary>
	Fixed = 1,
	/// <summary>
	/// Trailing order from v3 (moves to StopPx +/- Close * PegOffset
	/// </summary>
	Approaching = 2
}