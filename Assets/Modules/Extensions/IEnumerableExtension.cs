using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtensionsModule
{
    public static class IEnumerableExtension
    {
        /// <summary>
        /// Finds all the unique items inside the given collection.
        /// </summary>
        /// <param name="array">Collection to search through</param>
        /// <param name="action">Converter for the search</param>
        /// <typeparam name="T">Initial type of the items</typeparam>
        /// <typeparam name="U">New type of the items</typeparam>
        /// <returns>Collection of all the unique items</returns>
        public static IEnumerable<U> GetUniques<T, U>(this IEnumerable<T> array, Converter<T, U> action)
        {
            // Skip if action is invalid
            if (action == null)
                return Enumerable.Empty<U>();

            HashSet<U> unique = new();
            
            foreach (T item in array)
                unique.Add(action.Invoke(item));

            return unique.ToArray();
        }
    }
}