using System;
using System.Collections.Generic;
using System.Linq;

namespace QuantInfra.Sdk.StaticData.Synthetics;

public class CompositionUpdate
{
    public CompositionUpdate(SyntheticContractComposition current, SyntheticContractComposition @new, bool requiresBarsReload, double currentPrice)
    {
        throw new NotImplementedException(); // TODO
        // Current = current;
        // New = @new;
        // RequiresBarsReload = requiresBarsReload;
        // Delta = current.Weights
        //     .FullOuterJoinWithNulls(
        //         @new.Weights,
        //         w => w.Key,
        //         w => w.Key,
        //         (l, r, k) => new
        //         {
        //             ContractId = k, 
        //             Delta = new Tuple<decimal, decimal>(l?.Value ?? 0, r?.Value ?? 0)
        //         }
        //     )
        //     .Where(x => x.Delta.Item1 != x.Delta.Item2)
        //     .ToDictionary(x => x.ContractId, x => x.Delta);
        //
        // CurrentPrice = currentPrice;
    }

    public CompositionUpdate(CompositionUpdate other)
    {
        Current = other.Current;
        New = other.New;
        Delta = other.Delta.ToDictionary(kv => kv.Key, kv => kv.Value);
        RequiresBarsReload = other.RequiresBarsReload;
        CurrentPrice = other.CurrentPrice;
        NewPrice = other.NewPrice;
    }

    public SyntheticContractComposition Current { get; }
    public SyntheticContractComposition New { get; init; }
    public IReadOnlyDictionary<int, Tuple<decimal, decimal>> Delta { get; }
    public bool RequiresBarsReload { get; }
    public double CurrentPrice { get; }
    public double NewPrice { get; set; }

    protected bool Equals(CompositionUpdate other)
    {
        return Current.Equals(other.Current) 
               && New.Equals(other.New) 
               && RequiresBarsReload == other.RequiresBarsReload
               && Delta.Count == other.Delta.Count
               && !Delta.Keys.Except(other.Delta.Keys).Any()
               && Delta.Keys.All(i => Delta[i].Item1 == other.Delta[i].Item1 && Delta[i].Item2 == other.Delta[i].Item2);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CompositionUpdate)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Current, New, Delta, RequiresBarsReload);
    }
}