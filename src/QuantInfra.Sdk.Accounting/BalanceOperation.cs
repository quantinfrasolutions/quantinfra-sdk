using System;
using NodaTime;

namespace QuantInfra.Sdk.Accounting
{
	public class BalanceOperation
	{
        public int BalanceOperationId { get; init; }			
        public string AccountServiceName { get; init; }
        public int AccountId { get; init; }
        public Instant Dt { get; init; }
        public decimal Amount { get; init; }
        public int AssetId { get; init; }
        public decimal Price { get; init; }
        public decimal FxRate { get; init; }
        public decimal ValueInAccountCcy { get; init; }
        public string? ExternalId { get; init; }
        public string? Description { get; init; }
        public bool IsCorrection { get; init; }
        public bool AffectsPnL { get; init; }
        public bool AffectsInvestment { get; init; }
        public bool AffectsBalance { get; init; }
        public bool AffectsShareCount { get; init; }


        public BalanceOperation() { }

        public BalanceOperation(NewBalanceOperation bo, string accountServiceName, int balanceOperationBalanceOperationId, Instant dt, decimal fxRate, decimal valueInAccountCcy)
        {
            BalanceOperationId = balanceOperationBalanceOperationId;
            AccountServiceName = accountServiceName;
            AccountId = bo.AccountId;
            Dt = dt;
            Amount = bo.Amount;
            AssetId = bo.AssetId;
            Price = bo.Price;
            FxRate = fxRate;
            ValueInAccountCcy = valueInAccountCcy;
            ExternalId = bo.ExternalId;
            Description = bo.Description;
            IsCorrection = bo.IsCorrection;
            AffectsPnL = bo.AffectsPnL;
            AffectsInvestment = bo.AffectsInvestment;
            AffectsBalance = bo.AffectsBalance;
            AffectsShareCount = bo.AffectsShareCount;
        }

        public BalanceOperation(BalanceOperation bo)
		{
            BalanceOperationId = bo.BalanceOperationId;
            AccountServiceName = bo.AccountServiceName;
            AccountId = bo.AccountId;
            Dt = bo.Dt;
            Amount = bo.Amount;
            AssetId = bo.AssetId;
            Price = bo.Price;
            FxRate = bo.FxRate;
            ValueInAccountCcy = bo.ValueInAccountCcy;
            ExternalId = bo.ExternalId;
            Description = bo.Description;
            IsCorrection = bo.IsCorrection;
            AffectsPnL = bo.AffectsPnL;
            AffectsInvestment = bo.AffectsInvestment;
            AffectsBalance = bo.AffectsBalance;
            AffectsShareCount = bo.AffectsShareCount;
		}

		public BalanceOperation(string accountServiceName, int balanceOperationId, int accountId, Instant dt, decimal amount, int assetId, decimal price,
			decimal fxRate, decimal valueInAccountCcy, string? externalId, string? description, bool isCorrection,
			bool affectsPnL, bool affectsInvestment, bool affectsBalance, bool affectsShareCount)
		{
			AccountServiceName = accountServiceName;
			BalanceOperationId = balanceOperationId;
			AccountId = accountId;
			Dt = dt;
			Amount = amount;
			AssetId = assetId;
			Price = price;
			FxRate = fxRate;
			ValueInAccountCcy = valueInAccountCcy;
			ExternalId = externalId;
			Description = description;
			IsCorrection = isCorrection;
			AffectsPnL = affectsPnL;
			AffectsInvestment = affectsInvestment;
			AffectsBalance = affectsBalance;
			AffectsShareCount = affectsShareCount;
		}

		protected bool Equals(BalanceOperation other)
		{
			return BalanceOperationId == other.BalanceOperationId && AccountId == other.AccountId && Dt.Equals(other.Dt) && Amount == other.Amount && AssetId == other.AssetId && Price == other.Price && FxRate == other.FxRate && ValueInAccountCcy == other.ValueInAccountCcy && ExternalId == other.ExternalId && Description == other.Description && IsCorrection == other.IsCorrection && AffectsPnL == other.AffectsPnL && AffectsInvestment == other.AffectsInvestment && AffectsBalance == other.AffectsBalance && AffectsShareCount == other.AffectsShareCount;
		}

		public override bool Equals(object? obj)
		{
			if (obj is null) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((BalanceOperation)obj);
		}

		public override int GetHashCode()
		{
			var hashCode = new HashCode();
			hashCode.Add(BalanceOperationId);
			hashCode.Add(AccountId);
			hashCode.Add(Dt);
			hashCode.Add(Amount);
			hashCode.Add(AssetId);
			hashCode.Add(Price);
			hashCode.Add(FxRate);
			hashCode.Add(ValueInAccountCcy);
			hashCode.Add(ExternalId);
			hashCode.Add(Description);
			hashCode.Add(IsCorrection);
			hashCode.Add(AffectsPnL);
			hashCode.Add(AffectsInvestment);
			hashCode.Add(AffectsBalance);
			hashCode.Add(AffectsShareCount);
			return hashCode.ToHashCode();
		}

		public override string ToString()
		{
			return
				$"{nameof(BalanceOperationId)}: {BalanceOperationId}, {nameof(AccountId)}: {AccountId}, {nameof(Dt)}: {Dt}, {nameof(Amount)}: {Amount}, {nameof(AssetId)}: {AssetId}, {nameof(Price)}: {Price}, {nameof(FxRate)}: {FxRate}, {nameof(ValueInAccountCcy)}: {ValueInAccountCcy}, {nameof(ExternalId)}: {ExternalId}, {nameof(Description)}: {Description}, {nameof(IsCorrection)}: {IsCorrection}, {nameof(AffectsPnL)}: {AffectsPnL}, {nameof(AffectsInvestment)}: {AffectsInvestment}, {nameof(AffectsBalance)}: {AffectsBalance}, {nameof(AffectsShareCount)}: {AffectsShareCount}";
		}
	}
}

