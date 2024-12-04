using System.Collections;
using System.Collections.Generic;

namespace ExtensionsModule
{
    public static class IListExtension
    {
        /// <summary>
        /// Swaps the position of the two given items.
        /// </summary>
        /// <param name="array">Collection to swap in</param>
        /// <param name="a">Index of the first item</param>
        /// <param name="b">Index of the second item</param>
        /// <returns>Succeed to swap the items</returns>
        public static bool Swap(this IList array, int a, int b)
        {
            // If indexes outside the array, skip
            if (a < 0 || b < 0 || a >= array.Count || b >= array.Count)
                return false;

            // If same position, skip
            if (a == b)
                return true;

            // Swap
            (array[a], array[b]) = (array[b], array[a]);
            return true;
        }

        /// <summary>
        /// Fetches a random item in the given collection.
        /// </summary>
        /// <param name="array">Collection to fetch from</param>
        /// <param name="index">Index of the item fetched</param>
        /// <typeparam name="T">Type of item</typeparam>
        /// <returns>Item fetched</returns>
        public static T Random<T>(this IList<T> array, out int index) 
        {
            int amount = array.Count;

            index = amount switch
            {
                0 => -1,
                1 => 0,
                _ => UtilsModule.Random.RandomToMax(array.Count)
            };

            // If outside array, return default
            if (index < 0 || index >= amount)
                return default;

            return array[index];
        }
    }
}