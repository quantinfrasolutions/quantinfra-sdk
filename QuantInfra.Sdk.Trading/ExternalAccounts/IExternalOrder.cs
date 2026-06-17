namespace QuantInfra.Sdk.Trading.ExternalAccounts;

public interface IExternalOrder
{
    string ExternalContractId { get; init; } 
    int AccountId { get; init; }
    // TODO ExecutionReport RejectOrder(RejectReason rejectReason, string rejectReasonStr, Instant processingDt);
}