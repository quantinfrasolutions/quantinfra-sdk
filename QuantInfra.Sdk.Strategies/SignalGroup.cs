using System;
using System.Collections.Generic;
using System.Linq;

namespace QuantInfra.Sdk.Strategies
{
    public class SignalGroup
    {
        public virtual Guid SignalGroupId { get; set; }
        public virtual Dictionary<int, decimal> BaseVolumes { get; set; }
        public virtual decimal Investment { get; set; }

        public SignalGroup()
        {
            
        }

        public SignalGroup(Guid signalGroupId, decimal investment, Dictionary<int, decimal> baseVolumes)
        {
            SignalGroupId = signalGroupId;
            BaseVolumes = baseVolumes;
            Investment = investment;
        }
        
        public SignalGroup(decimal investment, Dictionary<int, decimal> baseVolumes) : this(Guid.NewGuid(), investment, baseVolumes)
        { }

        public SignalGroup(SignalGroup sg)
        {
            SignalGroupId = sg.SignalGroupId;
            Investment = sg.Investment;
            BaseVolumes = sg.BaseVolumes.ToDictionary(s => s.Key, s => s.Value);
        }
    }
}
