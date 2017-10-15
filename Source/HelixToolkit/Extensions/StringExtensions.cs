// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Extension methods for strings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit
{
    using System.Collections;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// A regular expression containing "a one or more whitespaces" pattern.
        /// </summary>
        private static Regex oneOrMoreWhitespaces = new Regex(@"\s+");

        /// <summary>
        /// Splits the string on whitespace.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>Array of strings.</returns>
        public static string[] SplitOnWhitespace(this string input)
        {
            return oneOrMoreWhitespaces.Split(input.Trim());
        }

        /// <summary>
        /// Creates a string from the items in an enumerable.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>
        /// A string.
        /// </returns>
        public static string EnumerateToString(this IEnumerable items, string prefix = null, string separator = " ")
        {
            var builder = new StringBuilder();
            foreach (var item in items)
            {
                if (builder.Length > 0)
                {
                    builder.Append(separator);
                }

                if (prefix != null)
                {
                    builder.Append(prefix);
                }

                builder.Append(item);
            }

            return builder.ToString();
        }
    }
}