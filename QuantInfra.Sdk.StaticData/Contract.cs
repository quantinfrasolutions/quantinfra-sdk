using System;
using System.Collections.Generic;
using System.Linq;
using Common.Trading;
using NodaTime;
using QuantInfra.Sdk.StaticData.Synthetics;

namespace QuantInfra.Sdk.StaticData;

public class Contract : IContract
{
    public Contract() {}
        
    public Contract(int contractId,
        string ticker,
        ContractTemplate template,
        LocalDate? firstTradingDate,
        LocalDate? expirationDate,
        SyntheticContractType? syntheticContractType,
        bool? synthRequiresBarRecalculationAtRollover,
        ICollection<SyntheticContractComposition>? syntheticContractCompositionHistory,
        string? externalContractId,
        Asset? asset,
        string? description,
        ICollection<Stream> streams,
        int defaultDatafeedId
    )
    {
        ContractId = contractId;
        Ticker = ticker;
        Template = template;
        FirstTradingDate = firstTradingDate;
        ExpirationDate = expirationDate;
        SyntheticContractType = syntheticContractType;
        SynthRequiresBarRecalculationAtRollover = synthRequiresBarRecalculationAtRollover;
        SyntheticContractCompositionHistory = syntheticContractCompositionHistory;
        ExternalContractId = externalContractId;
        Asset = asset;
        Description = description;
        Streams = streams;
        DefaultDatafeedId = defaultDatafeedId;
    }

    public int ContractId { get; init; }
    public string Ticker { get; init; }
        
    public ContractTemplate Template { get; init; }
        
    public LocalDate? FirstTradingDate { get; init; }
    public LocalDate? ExpirationDate { get; init; }
        
    public SyntheticContractType? SyntheticContractType { get; init; }
    public bool? SynthRequiresBarRecalculationAtRollover { get; init; }
    public ICollection<SyntheticContractComposition>? SyntheticContractCompositionHistory { get; init; }
    public bool IsSynthetic() => SyntheticContractType.HasValue;
        
    public string? ExternalContractId { get; init; } // TODO: may have multiple mappings
    public Asset? Asset { get; init; }
    public string? Description { get; init; }
        
    private IPnLCalculator? _plCalculator = null;
    public IPnLCalculator PLCalculator
    {
        get
        {
            if (_plCalculator == null)
            {
                    
                _plCalculator = Template.PlCalculatorType switch
                {
                    PLCalculatorType.Default => new DefaultFuturesContractPLCalculator(Template.TickSize,
                        Template.TickSize,
                        Template.PriceQuotation, settlCurrencyPrecision: GetSettlementCurrencyPrecision()),
                    PLCalculatorType.DefaultFutures => new DefaultFuturesContractPLCalculator(Template.TickSize,
                        Template.TickValue!.Value, Template.PriceQuotation, settlCurrencyPrecision: GetSettlementCurrencyPrecision()),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return _plCalculator;
        }
    }
        
    public ICollection<Stream> Streams { get; init; }
    public int DefaultDatafeedId { get; init; }
    public Stream? DefaultStream => Streams.SingleOrDefault(s => s.DatafeedId == DefaultDatafeedId);
        

    public decimal NormalizePrice(decimal price) =>
        Math.Round(
            price / Template.TickSize,
            0,
            (System.MidpointRounding)3  // HACK: MidpointRounding.ToNegativeInfinity — somehow the enum member is not accessible
        ) * Template.TickSize; 

    public decimal NormalizePrice(double price) => NormalizePrice((decimal)price);

    public decimal NormalizeVolume(decimal volume, MidpointRounding roundng = (MidpointRounding)2 /* ToZero */) =>
        volume < Template.MinSize 
            ? 0
            : Template.MinSize + Template.SizeIncrement * Math.Round((volume - Template.MinSize) / Template.SizeIncrement, 0, roundng);

    public SyntheticContractComposition? GetCurrentSyntheticContractComposition(Instant dt) =>
        SyntheticContractCompositionHistory!
            .Where(h => !h.ValidFrom.HasValue ||  h.ValidFrom <= dt)
            .OrderByDescending(h => h.ValidFrom)
            .FirstOrDefault();
        
    public SyntheticContractComposition? GetNextSyntheticContractComposition(Instant dt) =>
        SyntheticContractCompositionHistory!
            .Where(h => h.ValidFrom > dt)
            .OrderBy(h => h.ValidFrom)
            .FirstOrDefault();

    public Instant GetNextRolloverDt(Instant dt) =>
        SyntheticContractCompositionHistory!
            .OrderBy(c => c.ValidFrom)
            .FirstOrDefault(c => c.ValidFrom > dt)?
            .ValidFrom ?? Instant.MaxValue;

    public CompositionUpdate GetRolloverDeltas(Instant dt, double currentPrice = 0) =>
        new(
            GetCurrentSyntheticContractComposition(dt.Minus(Duration.Epsilon))!, 
            GetCurrentSyntheticContractComposition(dt)!, 
            SynthRequiresBarRecalculationAtRollover!.Value,
            currentPrice
        );

    public int GetSettlementCurrencyPrecision() =>
        Template.SettlementCurrency.BrokerOverrides?
            .SingleOrDefault(bo => bo.BrokerId == Template.Broker.BrokerId)?.Decimals
        ?? Template.SettlementCurrency.Decimals;
}