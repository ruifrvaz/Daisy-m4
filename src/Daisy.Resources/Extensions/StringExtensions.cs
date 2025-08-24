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
            if (start == -1) return null;

            start += prefix.Length;

            int end = output.IndexOf('>', start);
            if (end == -1) end = output.Length;

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

        public static bool ContainsCSharp(this string text)
        {
            int startIndex = text.IndexOf("```csharp");
            int endIndex = text.LastIndexOf("```");

            if (startIndex != -1 && endIndex != -1 && startIndex != endIndex)
            {
                return true;
            }
            return false;
        }

        public static string UnwrapCSharp(this string text)
        {
            int startIndex = text.IndexOf("```csharp");
            int endIndex = text.LastIndexOf("```");

            if (startIndex != -1 && endIndex != -1 && startIndex != endIndex)
            {
                var cSharpText = text.Substring(startIndex + 9, endIndex - startIndex - 9).Trim();
                if (!cSharpText.IsCSharpCompilable())
                {
                    return text;
                }

                return cSharpText;
            }
            else
            {
                return text;
            }
        }

        public static bool IsCSharpCompilable(this string text)
        {
            string pattern = @"^(?:using.*?;)?\s*.*?\bnamespace\b.*?\b(class|interface|abstract|enum)\b.*?}\s*}\s*$";

            return Regex.IsMatch(text, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        public static bool IsCSharpCompilable2(this string text)
        {
            // Matches the exact method signature with balanced braces for the method body
            string pattern = @"public\s+override\s+async\s+Task\s+Traverse\s*\(\s*Impulse\s+impulse\s*\)\s*\{(?:[^{}]*|(?<open>\{)|(?<-open>\}))*(?(open)(?!))\}";

            return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        //TODO: this will break when i add more jsons to the impulse.Output 
        public static T GetData<T>(this string data)
        {
            try
            {
                int jsonStartIndex = data.IndexOf("{");
                int jsonEndIndex = data.LastIndexOf("}");

                // Check if both start and end indices are found
                if (jsonStartIndex != -1 && jsonEndIndex != -1 && jsonStartIndex < jsonEndIndex)
                {
                    string jsonString = data.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex + 1).Trim();

                    var responseObject = JsonSerializer.Deserialize<T>(jsonString);

                    if (responseObject != null)
                    {
                        return responseObject;
                    }
                }

            }
            catch (JsonException)
            {
                return default;
            }

            return default;
        }

        public static bool TryGetData<T>(this string data, out T result)
        {
            result = default;

            try
            {
                int jsonStartIndex = data.IndexOf("{");
                int jsonEndIndex = data.LastIndexOf("}");

                if (jsonStartIndex != -1 && jsonEndIndex != -1 && jsonStartIndex < jsonEndIndex)
                {
                    string jsonString = data.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex + 1).Trim();

                    result = JsonSerializer.Deserialize<T>(jsonString);

                    return result != null;
                }
            }
            catch (JsonException)
            {
                // Ignore and fall through
            }

            return false;
        }

        public static string FirstLetterUpperCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            char[] charArray = input.ToCharArray();
            charArray[0] = char.ToUpper(charArray[0]);
            return new string(charArray);
        }
    }
}
