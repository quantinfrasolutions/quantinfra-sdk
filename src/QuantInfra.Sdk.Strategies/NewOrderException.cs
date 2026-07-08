using System;

namespace QuantInfra.Sdk.Strategies;

public class NewOrderException : Exception
{
    public NewOrderException(OrderExceptionReason reason = OrderExceptionReason.NotSpecified)
    {
        Reason = reason;
    }
    
    public OrderExceptionReason Reason { get; }
}

public enum OrderExceptionReason
{
    NotSpecified,
    ZeroQuantity,
    DuplicateClOrdId,
}