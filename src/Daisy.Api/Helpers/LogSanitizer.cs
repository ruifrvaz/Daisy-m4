// Copyright (c) 2025 Rui Filipe Rodrigues Vaz
// Licensed under the Apache License, Version 2.0

using System.Text.RegularExpressions;

namespace Daisy.Api.Helpers
{
    /// <summary>
    /// Helper class for sanitizing user input before logging to prevent log forging attacks.
    /// </summary>
    public static class LogSanitizer
    {
        private static readonly Regex NewLinePattern = new Regex(@"[\r\n]", RegexOptions.Compiled);

        /// <summary>
        /// Sanitizes a string value by removing or escaping characters that could be used for log forging.
        /// Removes newline characters that could be used to inject fake log entries.
        /// </summary>
        /// <param name="value">The value to sanitize.</param>
        /// <returns>A sanitized version of the input safe for logging.</returns>
        public static string Sanitize(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            // Remove newline characters to prevent log injection
            return NewLinePattern.Replace(value, " ");
        }

        /// <summary>
        /// Sanitizes a GUID by converting it to string and ensuring it's a valid GUID format.
        /// </summary>
        /// <param name="value">The GUID to sanitize.</param>
        /// <returns>The GUID as a string.</returns>
        public static string Sanitize(Guid value)
        {
            return value.ToString();
        }
    }
}
