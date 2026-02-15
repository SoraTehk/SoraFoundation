using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SoraTehk.Extensions {
    public static partial class CollectionX {
        #region GetOrAdd
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetOrAdd<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key
        ) where TValue : new() {
            //
            if (!dictionary.TryGetValue(key, out var value)) {
                value = new TValue();
                dictionary[key] = value;
            }

            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue GetOrAdd<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TValue> valueFactory
        ) { //
            if (!dictionary.TryGetValue(key, out var value)) {
                value = valueFactory(key);
                dictionary[key] = value;
            }

            return value;
        }
        #endregion
    }
}