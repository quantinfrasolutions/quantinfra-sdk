using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace QuantInfra.Sdk.StaticData.Synthetics;

public class SyntheticContractComposition
{
    private readonly IReadOnlyDictionary<int, double> _initialPrices = new Dictionary<int, double>();
    private readonly IReadOnlyDictionary<int, decimal> _weights = new Dictionary<int, decimal>();
    private readonly double? _initialPrice;

    public SyntheticContractComposition() { }

    public SyntheticContractComposition(SyntheticContractComposition other)
    {
        ValidFrom = other.ValidFrom;
        Weights = other.Weights.ToDictionary(kv => kv.Key, kv => kv.Value);
        InitialPrices = other.InitialPrices.ToDictionary(kv => kv.Key, kv => kv.Value);
    }
    
    public virtual Instant? ValidFrom { get; init; }

    public virtual double? InitialPrice
    {
        get => _initialPrice;
        init
        {
            _initialPrice = value;
            RequiresInitialization = GetIsNotInitialized();
        }
    }

    public virtual double InitialOffset { get; private init; }

    public virtual IReadOnlyDictionary<int, decimal> Weights
    {
        get => _weights;
        init
        {
            _weights = value;
            InitialOffset = CalculateInitialOffset();
        }
    }

    public virtual IReadOnlyDictionary<int, double> InitialPrices
    {
        get => _initialPrices;
        init
        {
            _initialPrices = value;
            InitialOffset = CalculateInitialOffset();
            RequiresInitialization = GetIsNotInitialized();
        }
    }
    
    public virtual bool RequiresInitialization { get; private init; }

    private double CalculateInitialOffset()
    {
        if (_initialPrices.Count > 0 && _weights.Count > 0)
        {
            return _initialPrices.Sum(kv => kv.Value * (double)_weights[kv.Key]);
        }

        return 0;
    }

    private bool GetIsNotInitialized() => !(InitialPrice.HasValue
        && _initialPrices.Count == _weights.Count && !_initialPrices.Keys.Except(_weights.Keys).Any());

    protected bool Equals(SyntheticContractComposition other)
    {
        return ValidFrom.Equals(other.ValidFrom) 
               && Weights.Count == other.Weights.Count
               && !Weights.Keys.Except(other.Weights.Keys).Any()
               && Weights.Keys.All(i => Weights[i] == other.Weights[i])
               && InitialPrices.Count == other.InitialPrices.Count
               && !InitialPrices.Keys.Except(other.InitialPrices.Keys).Any()
               && InitialPrices.Keys.All(i => InitialPrices[i] == other.InitialPrices[i])
               && RequiresInitialization == other.RequiresInitialization;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SyntheticContractComposition)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ValidFrom, Weights);
    }
}