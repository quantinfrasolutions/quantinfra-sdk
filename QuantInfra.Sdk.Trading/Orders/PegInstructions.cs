namespace QuantInfra.Sdk.Trading.Orders;

public record struct PegInstructions(
	decimal PegOffsetValue,
	PegMoveType PegMoveType
);