using System.Runtime.InteropServices;

namespace A3.RouteSearchGraphs.Extensions;

internal static class DictionaryExtensions
{
    extension<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        where TKey : notnull
        where TValue : notnull
    {
        internal TValue AddOrUpdate(TKey key, TValue value)
        {
            ref TValue? valueRef = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out _);

            valueRef = value;

            return valueRef;
        }
    }
}