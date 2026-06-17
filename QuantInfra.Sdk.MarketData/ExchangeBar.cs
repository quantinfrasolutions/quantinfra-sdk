using NodaTime;

namespace QuantInfra.Sdk.MarketData;

public class ExchangeBar
{
    public ExchangeBar(int streamId, int? contractId, Instant openDt, Instant closeDt, double open, double high, double low, double close, double volume, double dollarValue, int datasourceId, int? tradingSessionId)
    {
        StreamId = streamId;
        ContractId = contractId;
        OpenDt = openDt;
        CloseDt = closeDt;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
        DollarValue = dollarValue;
        DatasourceId = datasourceId;
        TradingSessionId = tradingSessionId;
    }

    public ExchangeBar(ExchangeBar bar)
    {
        StreamId = bar.StreamId;
        ContractId = bar.ContractId;
        OpenDt = bar.OpenDt;
        CloseDt = bar.CloseDt;
        Open = bar.Open;
        High = bar.High;
        Low = bar.Low;
        Close = bar.Close;
        Volume = bar.Volume;
        DollarValue = bar.DollarValue;
        DatasourceId = bar.DatasourceId;
        TradingSessionId = bar.TradingSessionId;
    }

    public int StreamId { get; init; }
    public int? ContractId { get; init; }
    public Instant OpenDt { get; init; }
    public Instant CloseDt { get; init; }
    public double Open { get; init; }
    public double High { get; init; }
    public double Low { get; init; }
    public double Close { get; init; }
    public double Volume { get; init; }
    public double DollarValue { get; init; }
    public int DatasourceId { get; init; }
    public int? TradingSessionId { get; init; }

    public override string ToString()
    {
        return $"{{ ExchangeBar | StreamId: {StreamId}, ContractId: {ContractId}, OpenDt: {OpenDt}, CloseDt: {CloseDt}, Open: {Open}, High: {High}, Low: {Low}, Close: {Close}, Volume: {Volume}, DollarValue: {DollarValue}, DatasourceId: {DatasourceId}, TradingSessionId: {TradingSessionId} }}";
    }

    protected bool Equals(ExchangeBar other)
    {
        return StreamId == other.StreamId && ContractId == other.ContractId && OpenDt.Equals(other.OpenDt) && CloseDt.Equals(other.CloseDt) && Open.Equals(other.Open) && High.Equals(other.High) && Low.Equals(other.Low) && Close.Equals(other.Close) && Volume.Equals(other.Volume) && DollarValue.Equals(other.DollarValue) && DatasourceId == other.DatasourceId && TradingSessionId == other.TradingSessionId;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ExchangeBar)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(StreamId);
        hashCode.Add(ContractId);
        hashCode.Add(OpenDt);
        hashCode.Add(CloseDt);
        hashCode.Add(Open);
        hashCode.Add(High);
        hashCode.Add(Low);
        hashCode.Add(Close);
        hashCode.Add(Volume);
        hashCode.Add(DollarValue);
        hashCode.Add(DatasourceId);
        hashCode.Add(TradingSessionId);
        return hashCode.ToHashCode();
    }
}