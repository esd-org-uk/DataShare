using System;
using System.Text.RegularExpressions;

namespace DS.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Null if the string is empty, otherwise the original string.
        /// (Useful to use with with null coalesce, e.g. myString.AsNullIfEmpty() ?? defaultString
        /// </summary>
        public static string AsNullIfEmpty(this string items)
        {
            return string.IsNullOrEmpty(items) ? null : items;
        }

        /// <summary>
        /// Null if the string is empty or whitespace, otherwise the original string.
        /// (Useful to use with with null coalesce, e.g. myString.AsNullIfWhiteSpace() ?? defaultString
        /// </summary>
        public static string AsNullIfWhiteSpace(this string items)
        {
            return string.IsNullOrWhiteSpace(items) ? null : items;
        }

        /// <summary>
        /// Creates a URL friendly slug from a string
        /// </summary>
        public static string ToUrlSlug(this string str)
        {
            // Repalce any characters that are not alphanumeric with hypen
            if (str == null) return "";
            if (str == "") return "";
            str = Regex.Replace(str, "[^a-z^0-9]", "-", RegexOptions.IgnoreCase);

            // Replace all double hypens with single hypen
            var pattern = "--";
            while (Regex.IsMatch(str, pattern))
                str = Regex.Replace(str, pattern, "-", RegexOptions.IgnoreCase);

            // Remove leading and trailing hypens ("-")
            pattern = "^-|-$";
            str = Regex.Replace(str, pattern, "", RegexOptions.IgnoreCase);

            return str.ToLower();
        }

        public static string RemovePunctuationAndSpacing(this string s, bool toLower)
        {
            if (!String.IsNullOrEmpty(s))
            {
                return toLower ? Regex.Replace(s, "[^a-zA-Z0-9]", "").ToLower() : Regex.Replace(s, "[^a-zA-Z0-9]", "");
            }
            return "";
        }
        
        public static string RemovePunctuationAndSpacing(this string s)
        {
            return RemovePunctuationAndSpacing(s, true);
        }

        public static string FormatCurrency(string amount)
        {
            var amt = amount.Replace("£", "");
            var valArray = amt.Split(',');

            if (valArray.Length == 3)//millions
            {
                var millions = valArray[0];
                var thousands = Convert.ToInt32(valArray[1]) == 0 ? "" : "." + valArray[1].Substring(0, 2);
                return string.Format("£{0}{1}m", millions, thousands);
            }
            if (valArray.Length == 2) //thousands
            {
                return string.Format("£{0}k", valArray[1]);
            }
            return string.Format("£{0}", valArray[0]);
        }

        public static string ConvertBytesToString(this long bytes)
        {
            const int scale = 1024;
            var orders = new[] { "GB", "MB", "KB", "B" };
            var max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (var order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.#} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 B";
        } 
    }
}
