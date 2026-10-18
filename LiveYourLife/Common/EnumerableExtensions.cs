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
    
    public static void RemoveRange<TElement>(this ICollection<TElement> enumerable, IEnumerable<TElement> toRemoves)
    {
        foreach (var toRemove in toRemoves)
        {
            enumerable.Remove(toRemove);
        }
    }
    
    public static IEnumerable<T> Shuffled<T>(this IEnumerable<T> source)
    {
        var items = source.ToArray();

        for (var i = items.Length - 1; i > 0; i--)
        {
            var j = Random.Next(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }

        return items;
    }
}