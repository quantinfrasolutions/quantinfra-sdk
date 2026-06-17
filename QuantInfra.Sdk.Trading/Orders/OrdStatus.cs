namespace QuantInfra.Sdk.Trading.Orders;

public enum OrdStatus
{        
    New = 0,
    PartiallyFilled = 1,
    Filled = 2,        
    Canceled = 4,   
    PendingCancel = 6,
    Rejected = 8,
    Suspended = 9,
    PendingNew = 0xA,
    PendingReplace = 0xE,
    Split = 0xF,
}

public static class OrdStatusExtensions
{
    public static bool IsTerminal(this OrdStatus s) => s == OrdStatus.Canceled || s == OrdStatus.Filled || s == OrdStatus.Rejected || s == OrdStatus.Split;

    public static bool IsPending(this OrdStatus s) => s == OrdStatus.PendingNew || s == OrdStatus.PendingReplace ||
                                                      s == OrdStatus.PendingCancel;
}