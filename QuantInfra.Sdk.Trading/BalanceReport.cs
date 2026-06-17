using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Trading
{
    public class BalanceReport
    {
        public BalanceReport() { }

        public BalanceReport(BalanceReport r)
        {
            AccountId = r.AccountId;
            Exchange = r.Exchange;
            Balance = r.Balance.ToDictionary(i => i.Key, i => i.Value);
            Dt = r.Dt;
            IsSnapshot = r.IsSnapshot;
        }

        public string AccountId { get; set; }
        public string Exchange { get; set; }
        public Dictionary<string, decimal> Balance { get; set; }
        public DateTime Dt { get; set; }
        public bool IsSnapshot { get; set; }

        public string InternalAccountId => $"{Exchange}.{AccountId}";


        public override string ToString() => $"InternalAccountId={InternalAccountId}, Dt={Dt}, IsSnapshot={IsSnapshot}, " +
            $"Balance=[{string.Join(',', Balance.Select(b => $"{b.Key}={b.Value}"))}]";
    }
}
