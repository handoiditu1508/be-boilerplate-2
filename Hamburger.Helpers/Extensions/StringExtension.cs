using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Hamburger.Helpers.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Indicates whether the specified string is null or an empty string ("").
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <returns>true if the value parameter is null or System.String.Empty, or if value consists exclusively of white-space characters.</returns>
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// Upper case first letter of string.
        /// </summary>
        /// <param name="value">String to upper case.</param>
        /// <returns>String with upper cased first letter.</returns>
        public static string UpperCaseFirstLetter(this string value)
        {
            switch (value)
            {
                case null:
                    throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(value));
                case "":
                    return string.Empty;
                default:
                    return string.Concat(value[0].ToString().ToUpper(), value.AsSpan(1));
            }
        }

        /// <summary>
        /// Converts the specified string to title case (except for words that are entirely in uppercase, which are considered to be acronyms).
        /// </summary>
        /// <param name="value">The string to convert to title case.</param>
        /// <returns>The specified string converted to title case.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToTitleCase(this string value)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(value);
        }

        /// <summary>
        /// Remove special characters from string, valid characters regex is [^a-zA-Z0-9_.]+ .
        /// </summary>
        /// <param name="value">String to evaluate.</param>
        /// <returns>String with special characters removed</returns>
        public static string RemoveSpecialCharacters(this string value)
        {
            var sb = new StringBuilder();
            foreach (char c in value)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Indicates whether the specified Unicode character is categorized as a letter or a decimal digit.
        /// </summary>
        /// <param name="value">String to evaluate.</param>
        /// <returns>true if string only hold alphanumeric characters; otherwise, false.</returns>
        public static bool IsAlphanumeric(this string value) => value.All(char.IsLetterOrDigit);

        /// <summary>
        /// Create a stream from current string.
        /// </summary>
        /// <param name="value">String to create stream from.</param>
        /// <returns>A stream of the string value.</returns>
        public static Stream ToStream(this string value)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(value);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
