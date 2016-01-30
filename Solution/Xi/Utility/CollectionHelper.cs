using System;
using System.Collections.Generic;

namespace Xi
{
    /// <summary>
    /// Provides methods for operating on collections.
    /// </summary>
    public static class CollectionHelper
    {
        /// <summary>
        /// Is the sequence empty?
        /// </summary>
        public static bool Empty<T>(this IEnumerable<T> enumerable) where T : class
        {
            XiHelper.ArgumentNullCheck(enumerable);
            return !enumerable.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Test if a sequence has one item.
        /// </summary>
        public static bool HasOne<T>(this IEnumerable<T> enumerable) where T : class
        {
            return enumerable.HasCount(1);
        }

        /// <summary>
        /// Test if a sequence has the given count.
        /// </summary>
        public static bool HasCount<T>(this IEnumerable<T> enumerable, int count) where T : class
        {
            int counted = 0;
            foreach (T item in enumerable)
                if (++counted > count) // OPTIMIZATION: early return
                    return false;
            return counted == count;
        }

        /// <summary>
        /// Assign the items of a list.
        /// </summary>
        public static void Assign<T, U>(this IList<T> list, List<U> source) where U : T
        {
            XiHelper.ArgumentNullCheck(list, source);
            list.Clear();
            foreach (T item in source) list.Add(item);
        }

        /// <summary>
        /// Add items to a list.
        /// </summary>
        public static void Add<T, U>(this IList<T> list, List<U> source) where U : T
        {
            XiHelper.ArgumentNullCheck(list, source);
            foreach (U item in source) list.Add(item);
        }

        /// <summary>
        /// Add items to a list.
        /// </summary>
        public static void Add<T, U>(this IList<T> list, Dictionary<U, U> source) where U : T
        {
            XiHelper.ArgumentNullCheck(list, source);
            foreach (U item in source.Values) list.Add(item);
        }

        /// <summary>
        /// Get the first item in a list.
        /// Constant-time and doesn't allocate.
        /// </summary>
        public static T First<T>(this IList<T> list)
        {
            XiHelper.ArgumentNullCheck(list);
            return list[0];
        }

        /// <summary>
        /// Get the first item in a list, or default(T) if there is none.
        /// Constant-time and doesn't allocate.
        /// </summary>
        public static T FirstOrDefault<T>(this IList<T> list)
        {
            XiHelper.ArgumentNullCheck(list);
            return list.Count != 0 ? list.First() : default(T);
        }

        /// <summary>
        /// Get the first value in a pseudo-set.
        /// Constant-time and doesn't allocate.
        /// </summary>
        public static T First<T>(this Dictionary<T, T> pset)
        {
            XiHelper.ArgumentNullCheck(pset);
            foreach (T value in pset.Values) return value;
            throw new ArgumentException("Cannot get the first value of an empty pseudo-set.");
        }

        /// <summary>
        /// Get the first value in a pseudo-set, or default(T) if there is none.
        /// Constant-time and doesn't allocate.
        /// </summary>
        public static T FirstOrDefault<T>(this Dictionary<T, T> pset)
        {
            XiHelper.ArgumentNullCheck(pset);
            foreach (T value in pset.Values) return value;
            return default(T);
        }

        /// <summary>
        /// Get the last item in a list.
        /// Constant-time and doesn't allocate.
        /// </summary>
        public static T Last<T>(this IList<T> list)
        {
            XiHelper.ArgumentNullCheck(list);
            return list[list.Count - 1];
        }

        /// <summary>
        /// Get the last item in a list, or default(T) if there is none.
        /// Constant-time and doesn't allocate.
        /// </summary>
        public static T LastOrDefault<T>(this IList<T> list)
        {
            XiHelper.ArgumentNullCheck(list);
            return list.Count != 0 ? list.Last() : default(T);
        }

        /// <summary>
        /// Try to add an item to a dictionary.
        /// </summary>
        public static bool TryAddValue<K, V>(this IDictionary<K, V> dictionary, KeyValuePair<K, V> item)
        {
            return dictionary.TryAddValue(item.Key, item.Value);
        }

        /// <summary>
        /// Try to add a value to a dictionary.
        /// </summary>
        public static bool TryAddValue<K, V>(this IDictionary<K, V> dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key)) return false;
            dictionary.Add(key, value);
            return true;
        }

        /// <summary>
        /// Add a value to a pseudo-set.
        /// </summary>
        public static void Add<V>(this IDictionary<V, V> pset, V value)
        {
            pset.Add(value, value);
        }

        /// <summary>
        /// Try to add a value to a pseudo-set.
        /// </summary>
        public static bool TryAddValue<V>(this IDictionary<V, V> pset, V value)
        {
            if (pset.ContainsKey(value)) return false;
            pset.Add(value, value);
            return true;
        }
    }
}
