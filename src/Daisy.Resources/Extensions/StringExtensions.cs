using Daisy.Resources.Signals;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Daisy.Resources.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveSubstring(this string currentString, string stringToRemove, bool ignoreCase)
        {
            if (currentString == null)
            {
                throw new ArgumentNullException(nameof(currentString));
            }

            if (stringToRemove == null)
            {
                throw new ArgumentNullException(nameof(stringToRemove));
            }

            // Use Regex.Escape to handle special characters in the stringToRemove
            string pattern = Regex.Escape(stringToRemove);

            // Use RegexOptions to specify case-insensitive matching if needed
            RegexOptions options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;

            // Use Regex.Replace to remove the substring
            string finalString = Regex.Replace(currentString, pattern, string.Empty, options);

            return finalString;
        }

        public static string Merge(this List<string> stringList, string separator)
        {
            if (stringList == null)
            {
                throw new ArgumentNullException(nameof(stringList));
            }

            return string.Join(separator, stringList);
        }

        public static string GetChainByKey(this string output, string key)
        {
            if (string.IsNullOrEmpty(output) || string.IsNullOrEmpty(key))
                return null;

            string prefix = key + ":";

            // Case-insensitive search
            int start = output.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (start == -1)
                return null;

            start += prefix.Length;

            int end = output.IndexOf('>', start);
            if (end == -1)
                end = output.Length;

            return output.Substring(start, end - start).Trim();
        }

        public static string AddPrefix(this string originalString, string prefix)
        {
            // Check for null originalString
            if (originalString == null)
            {
                throw new ArgumentNullException(nameof(originalString));
            }

            // Use string.Concat to add the prefix
            return string.Concat(prefix, originalString);
        }

        public static string GetLastChain(this string output, string key)
        {
            if (string.IsNullOrEmpty(output) || string.IsNullOrEmpty(key))
            {
                return null;
            }

            string prefix = key + ":";

            int prefixIndex = output.LastIndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (prefixIndex == -1)
            {
                return null;
            }

            int start = output.LastIndexOf('<', prefixIndex);
            start = start == -1 ? prefixIndex : start + 1;

            int end = output.IndexOf('>', prefixIndex);
            if (end == -1)
            {
                end = output.Length;
            }

            var chain = output.Substring(start, end - start);

            return chain.Trim();
        }
    }
}
