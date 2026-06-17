using NodaTime;

namespace QuantInfra.Sdk.Accounting
{
	public class NewBalanceOperation
	{        
        public int AccountId { get; set; }
        public Instant? Dt { get; set; }
        public decimal Amount { get; set; }
        public int AssetId { get; set; }

        public decimal Price { get; set; }
        public decimal? FxRate { get; set; }

        public decimal? SharePrice { get; set; }

        public string? ExternalId { get; set; }
        public string? Description { get; set; }

        // TODO
        public bool DelayUntilNextEndOfDay { get; set; }
        public bool IsCorrection { get; set; } = false;
        public bool AffectsPnL { get; set; }
        public bool AffectsInvestment { get; set; } = true;
        public bool AffectsBalance { get; set; } = true;
        public bool AffectsShareCount { get; set; } = true;


        public override string ToString()
        {
            return $"{{ {nameof(NewBalanceOperation)} | {nameof(AccountId)}: {AccountId}, {nameof(Dt)}: {Dt}, {nameof(Amount)}: {Amount}, {nameof(AssetId)}: {AssetId}, {nameof(Price)}: {Price}, {nameof(FxRate)}: {FxRate}, {nameof(SharePrice)}: {SharePrice}, {nameof(ExternalId)}: {ExternalId}, {nameof(Description)}: {Description}, {nameof(IsCorrection)}: {IsCorrection}, {nameof(AffectsPnL)}: {AffectsPnL}, {nameof(AffectsInvestment)}: {AffectsInvestment}, {nameof(AffectsBalance)}: {AffectsBalance} }}";
        }
    }
}

