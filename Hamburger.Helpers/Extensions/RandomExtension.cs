using System;

namespace Hamburger.Helpers.Extensions
{
    public static class RandomExtension
    {
        /// <summary>
        /// Get a random string.
        /// </summary>
        /// <param name="random">An instance of Random.</param>
        /// <param name="lenght">Length of the result string.</param>
        /// <param name="allowedCharacters">Characters that are allowed.</param>
        /// <returns>String comprise of random characters.</returns>
        public static string NextString(this Random random, int lenght, string allowedCharacters = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-")
        {
            if (allowedCharacters.IsNullOrEmpty())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(allowedCharacters));

            char[] chars = new char[lenght];

            for (int i = 0; i < lenght; i++)
            {
                chars[i] = allowedCharacters[random.Next(0, allowedCharacters.Length)];
            }

            return new string(chars);
        }
    }
}
