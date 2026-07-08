using QuantInfra.Sdk.Trading;

namespace QuantInfra.Sdk.StaticData;

public static class PnLCalculatorExtensions
{
    public static PnLCalculatorOptions GetPnLCalculatorOptions(this ContractTemplate template) =>
        new(template.PnLCalculatorType, template.TickSize,
            template.TickValue ?? template.TickSize, template.SettlementCurrency.Decimals
        );
    
    public static IPnLCalculator GetCalculator(this ContractTemplate template) =>
        template.GetPnLCalculatorOptions().GetCalculator();
        

    public static IPnLCalculator GetCalculator(this Contract contract) => contract.Template.GetCalculator();
}