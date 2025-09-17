using System;
using System.Collections.Generic;
using Daisy.Resources.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daisy.Tests.Extensions
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void RemoveSubstring_WhenCurrentStringIsNull_ThrowsArgumentNullException()
        {
            string? currentString = null;

            Action action = () => currentString!.RemoveSubstring("test", ignoreCase: false);

            action.Should().Throw<ArgumentNullException>()
                .WithParameterName("currentString");
        }

        [TestMethod]
        public void RemoveSubstring_WhenSubstringContainsRegexCharacters_RemovesSubstringSafely()
        {
            const string currentString = "value.+to.+trim";

            var result = currentString.RemoveSubstring(".+", ignoreCase: false);

            result.Should().Be("valuetotrim");
        }

        [TestMethod]
        public void RemoveSubstring_WhenIgnoringCase_RemovesAllMatchingSubstrings()
        {
            const string currentString = "HelloWorldWORLD";

            var result = currentString.RemoveSubstring("world", ignoreCase: true);

            result.Should().Be("Hello");
        }

        [TestMethod]
        public void RemoveSubstring_WhenStringToRemoveIsNull_ThrowsArgumentNullException()
        {
            const string currentString = "sample";

            Action action = () => currentString.RemoveSubstring(null!, ignoreCase: false);

            action.Should().Throw<ArgumentNullException>()
                .WithParameterName("stringToRemove");
        }

        [TestMethod]
        public void Merge_WhenListIsNull_ThrowsArgumentNullException()
        {
            List<string>? strings = null;

            Action action = () => strings!.Merge(", ");

            action.Should().Throw<ArgumentNullException>()
                .WithParameterName("stringList");
        }

        [TestMethod]
        public void Merge_JoinsStringsWithSeparator()
        {
            var strings = new List<string> { "one", "two", "three" };

            var result = strings.Merge("|");

            result.Should().Be("one|two|three");
        }

        [TestMethod]
        public void GetChainByKey_WhenKeyExists_ReturnsTrimmedValue()
        {
            const string output = "<City: London><Country: United Kingdom>";

            var result = output.GetChainByKey("city");

            result.Should().Be("London");
        }

        [TestMethod]
        public void GetChainByKey_WhenKeyDoesNotExist_ReturnsNull()
        {
            const string output = "<city: London>";

            var result = output.GetChainByKey("country");

            result.Should().BeNull();
        }

        [TestMethod]
        public void AddPrefix_WhenOriginalStringIsNull_ThrowsArgumentNullException()
        {
            string? original = null;

            Action action = () => original!.AddPrefix("prefix");

            action.Should().Throw<ArgumentNullException>()
                .WithParameterName("originalString");
        }

        [TestMethod]
        public void AddPrefix_WhenPrefixProvided_ReturnsConcatenatedString()
        {
            const string original = "value";

            var result = original.AddPrefix("prefix-");

            result.Should().Be("prefix-value");
        }

        [TestMethod]
        public void GetLastChain_WhenKeyExists_ReturnsLastChain()
        {
            const string output = "<city: Paris><city: Madrid><country: Spain>";

            var result = output.GetLastChain("city");

            result.Should().Be("city: Madrid");
        }

        [TestMethod]
        public void GetLastChain_WhenKeyDoesNotExist_ReturnsNull()
        {
            const string output = "<city: Paris>";

            var result = output.GetLastChain("country");

            result.Should().BeNull();
        }
    }
}
