using System.Collections.Generic;
using QuantInfra.Sdk.Trading;

namespace QuantInfra.Sdk.StaticData;

public class ContractTemplate
{
	public ContractTemplate() { }
	
	public ContractTemplate(int templateId, string name, SecurityType securityType, Asset? asset, decimal minSize,
		decimal? minSizeMoney, decimal maxSize, decimal? maxSizeMoney, decimal sizeIncrement, decimal tickSize,
		decimal? tickValue, decimal priceQuotation, Currency settlementCurrency, PnLCalculatorType pnLCalculatorType,
		Currency? baseCurrency, Currency? quoteCurrency, Datafeed? defaultDatafeed,
		ICollection<CommissionStructure> commissions, ICollection<TradingSession> tradingSessions, Exchange exchange,
		Broker broker, int daysInYear, string? description)
	{
		TemplateId = templateId;
		Name = name;
		SecurityType = securityType;
		Asset = asset;
		MinSize = minSize;
		MinSizeMoney = minSizeMoney;
		MaxSize = maxSize;
		MaxSizeMoney = maxSizeMoney;
		SizeIncrement = sizeIncrement;
		TickSize = tickSize;
		TickValue = tickValue;
		PriceQuotation = priceQuotation;
		SettlementCurrency = settlementCurrency;
		PnLCalculatorType = pnLCalculatorType;
		BaseCurrency = baseCurrency;
		QuoteCurrency = quoteCurrency;
		DefaultDatafeed = defaultDatafeed;
		Commissions = commissions;
		TradingSessions = tradingSessions;
		Exchange = exchange;
		Broker = broker;
		DaysInYear = daysInYear;
		Description = description;
	}

	public int TemplateId { get; init; }
	public string Name { get; init; }
	public SecurityType SecurityType { get; init; }
	public PnLCalculatorType PnLCalculatorType { get; init; } = PnLCalculatorType.Default;
	public Asset? Asset { get; init; }
	public decimal MinSize { get; init; } = 1;
	public decimal? MinSizeMoney { get; init; }
	public decimal MaxSize { get; init; } = 1000000m;
	public decimal? MaxSizeMoney { get; init; }
	public decimal SizeIncrement { get; init; } = 1;
	public decimal TickSize { get; init; } = 0.01m;
	public decimal? TickValue { get; init; }
	public decimal PriceQuotation { get; init; } = 1;
	public Currency SettlementCurrency { get; init; }
	public Currency? BaseCurrency { get; init; }
	public Currency? QuoteCurrency { get; init; }
	public Datafeed? DefaultDatafeed { get; init; }
	public ICollection<CommissionStructure> Commissions { get; init; }
	public ICollection<TradingSession> TradingSessions { get; init; }
	public Exchange Exchange { get; init; } // TODO: contract may be listed on several exchanges, with its own commission structures and trading sessions
	public Broker Broker { get; init; } // TODO: contract may be provided by several brokers, with its own commission structures
	public int DaysInYear { get; init; } = 252;
	public string? Description { get; init; }
}