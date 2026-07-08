using System.Collections.Generic;
using System.Linq;

namespace QuantInfra.Sdk.Utils
{
    public static class CollectionsToStringExtensions
    {
        public static string ToFormattedString<TKey, TValue>(this Dictionary<TKey, TValue>? d) =>
            d == null ?
                "null" :
                "[" + string.Join(", ", d.Select(i => $"{i.Key}={i.Value}")) + "]";

        public static string ToFormattedString<TItem>(this IEnumerable<TItem>? e) =>
            e == null ?
                "null" :
                "[" + string.Join(", ", e) + "]";
    }
}