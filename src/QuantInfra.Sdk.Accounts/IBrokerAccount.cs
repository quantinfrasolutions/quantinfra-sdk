using System;
using NodaTime;
using QuantInfra.Sdk.Accounts.ExternalAccounts;
using QuantInfra.Sdk.Trading.ExternalAccounts;
using QuantInfra.Sdk.Trading.Orders;

namespace QuantInfra.Sdk.Accounts;

public interface IBrokerAccount : IAccount
{
    void OnExternalExecutionReport(ExternalExecutionReport externalER, Instant processingDt);
    void OnExternalOrderCancelReject(OrderCancelReject externalOcr, Instant processingDt);
    void OnExternalTrade(ExternalTradeRecord externalTradeRecord, Instant processingDt);
    void OnExternalPositionReport(ExternalPositionReport positionReport, Instant processingDt);
    void OnExternalBalanceOperation(ExternalBalanceOperation balanceOperation, Instant processingDt);
    
    void OnExternalAccountOrdersSnapshot(ExternalAccountOrdersSnapshot snapshot, Instant processingDt);
    void OnExternalAccountTradesReport(ExternalAccountTradesReport report, Instant processingDt);
    void OnExternalAccountPositionsSnapshot(AccountPositionsSnapshot snapshot, Instant processingDt);
     void OnExternalAccountBalancesSnapshot(AccountBalancesSnapshot snapshot, Instant processingDt);
    void OnFullSnapshot(ExternalAccountFullSnapshot snapshot, Instant processingDt);
    public void OnExecutionServiceMissedVersionEvt(Instant processingDt);
    public void OnExternalAccountConnectionRestoredEvt(Instant processingDt);
    
    void PlaceExternalOrder(ExecutionReport er, Instant processingDt);
    void CancelExternalOrder(ExecutionReport er, Instant processingDt);
    void ReplaceExternalOrder(OrderReplaceRequest req, ExecutionReport er, Instant processingDt);
    
    void Reconcile(Instant processingDt, Guid? requestId);
}