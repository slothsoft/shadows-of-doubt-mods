using System;
using System.Collections.Generic;
using System.Linq;

namespace LiveYourLife.Common;

public static class EnumerableExtensions
{
    private static readonly Random Random = new();

    public static TElement RandomElement<TElement>(this IEnumerable<TElement> enumerable)
    {
        var collection = enumerable.ToArray();
        return collection[Random.Next(collection.Length)];
    }

    public static TElement? RandomElementOrDefault<TElement>(this IEnumerable<TElement> enumerable,
        TElement? defaultValue = default)
    {
        var collection = enumerable.ToArray();
        if (collection.Length == 0) return defaultValue;
        return collection[Random.Next(collection.Length)];
    }
}