using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Diquis.Application.Common.Filter;

namespace Diquis.Application.Utility
{
    public static class NanoHelpers // helper utility methods
    {
        static Random random = new Random();
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
        public static string ReplaceWhitespace(this string input, string replacement)
        {
            return _whitespace.Replace(input, replacement);
        }


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
