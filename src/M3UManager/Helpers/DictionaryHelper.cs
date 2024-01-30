using System;
using System.Collections.Generic;

namespace M3UManager.Helpers;

internal static class DictionaryHelper
{
    internal static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        => dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
}
