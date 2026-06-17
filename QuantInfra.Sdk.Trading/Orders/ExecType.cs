namespace QuantInfra.Sdk.Trading.Orders;

public enum ExecType
{
	New = 0,
	Canceled = 4,
	Replaced = 5,
	PendingCancel = 6,
	Rejected = 8,
	PendingNew = 0xA,
	Restated = 0xD,
	PendingReplace = 0xE,
	Fill = 0xF,
	OrderStatus = 18,
	TriggeredOrActivatedBySystem = 0x15, // L
}