using System;

namespace Common.Trading;

public class Commission
{
    public long CommissionId { get; init; }
    public Guid TradeId { get; init; }
    public decimal Amount { get; init; }
}