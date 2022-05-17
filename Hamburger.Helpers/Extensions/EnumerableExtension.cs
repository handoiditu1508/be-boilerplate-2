using System;
using System.Collections.Generic;
using System.Linq;

namespace Hamburger.Helpers.Extensions
{
    public static class EnumerableExtension
    {
        /// <summary>
        /// Shuffle and return a new list from old one.
        /// </summary>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="enumeration">List to shuffle.</param>
        /// <returns>Shuffled list.</returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumeration)
        {
            var result = enumeration.ToList();

            var random = new Random();

            var currentIndex = result.Count;

            // While there remain elements to shuffle.
            while (currentIndex != 0)
            {
                // Pick a remaining element.
                var randomIndex = random.Next(currentIndex);
                currentIndex--;

                // And swap it with the current element.
                (result[randomIndex], result[currentIndex]) = (result[currentIndex], result[randomIndex]);
            }

            return result;
        }

        /// <summary>
        /// Performs the specified action on each element of the IEnumerable.
        /// Warning: do not pass async action to this function since action will run it synchonously.
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="enumeration">List of T.</param>
        /// <param name="action">The Action delegate to perform on each element of the IEnumerable.</param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (var item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// Indicates whether the specified enumeration is null or an empty enumeration ([]).
        /// </summary>
        /// <typeparam name="T">Type of enumeration.</typeparam>
        /// <param name="enumeration">The enumeration to test.</param>
        /// <returns>true if the enumeration parameter is null or an empty enumeration ([]); otherwise, false.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumeration)
        {
            if (enumeration == null || !enumeration.Any())
                return true;
            return false;
        }
    }
}
