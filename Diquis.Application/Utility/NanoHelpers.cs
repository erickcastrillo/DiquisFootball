using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Diquis.Application.Common.Filter;

namespace Diquis.Application.Utility
{
    /// <summary>
    /// Provides utility helper methods for string manipulation, enum description retrieval, 
    /// hex code generation, and order by string formatting.
    /// </summary>
    public static class NanoHelpers // helper utility methods
    {
        /// <summary>
        /// Static instance of Random to be used in hex generation.
        /// </summary>
        static Random random = new Random();
        
        /// <summary>
        /// Generates a random hexadecimal string of the specified number of digits.
        /// </summary>
        /// <param name="digits">The number of hexadecimal digits to generate.</param>
        /// <returns>A random hexadecimal string.</returns>
        public static string GenerateHex(int digits) // hex code generator
        {
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = string.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
            {
                return result;
            }

            return result + random.Next(16).ToString("X");
        }

        /// <summary>
        /// Retrieves the description attribute of an enum value, if present; otherwise returns the enum's name.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The description of the enum value or its name if no description is found.</returns>
        public static string GetEnumDescription(this Enum value) // retrieve enum descriptions
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }
        
        private static readonly Regex _whitespace = new(@"\s+"); // remove whitespace from strings
        
        /// <summary>
        /// Replaces all whitespace in the input string with the specified replacement string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="replacement">The string to replace whitespace with.</param>
        /// <returns>The modified string with whitespace replaced.</returns>
        public static string ReplaceWhitespace(this string input, string replacement)
        {
            return _whitespace.Replace(input, replacement);
        }

        /// <summary>
        /// Converts a string to a URL-friendly slug by lowercasing, removing accents, 
        /// replacing spaces, and removing invalid characters.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns>A URL slug generated from the input string.</returns>
        public static string ToUrlSlug(string value) // generate url slug from string
        {

            // first to lower case
            value = value.ToLowerInvariant();

            // remove all accents
            value = RemoveAccents(value);

            // replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            // remove invalid chars
            value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            // trim dashes from end
            value = value.Trim('-', '_');

            // replace double occurences of - or _
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }
        
        /// <summary>
        /// Removes accent characters from the input string.
        /// </summary>
        /// <param name="text">The input string.</param>
        /// <returns>The string with accents removed.</returns>
        public static string RemoveAccents(string text) // remove accents from string characters
        {
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    _ = stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        
        /// <summary>
        /// Generates an OrderBy string from the provided PaginationFilter.
        /// </summary>
        /// <param name="filter">The PaginationFilter containing sorting information.</param>
        /// <returns>A string representing the OrderBy clause.</returns>
        public static string GenerateOrderByString(PaginationFilter filter)
        {

            // translate a dynamic TanstackColumnOrder List into a string format readable by ardalis specification OrderBy
            // string format example: ('Name,-Supplier,Property.Name,Price') -prefix denotes Descending
            string sortingString = "";
            int numberOfColumns = filter.Sorting.Count;

            int count = 1;
            foreach (TanstackColumnOrder sortColumn in filter.Sorting)
            {
                if (sortColumn.Desc) // prepend a minus if order equals descending
                {
                    sortingString += "-" + sortColumn.Id;
                }
                else
                {
                    sortingString += sortColumn.Id;
                }

                if (count != numberOfColumns) // append comma if not last in series
                {
                    sortingString += ",";
                }
                count++;
            }
            return sortingString;
        }
    }
}
