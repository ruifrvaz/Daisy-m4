using FluentAssertions;
using Daisy.Resources.Extensions;
using System.Text.RegularExpressions;

namespace Daisy.Tests.Extensions.StringExtensionsTest
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void TestUnwrapText()
        {
            // Arrange
            string wrappedText = "```csharp using System; namespace Namespace { public class MyClass { } }```";
            string expectedText = "using System; namespace Namespace { public class MyClass { } }";

            // Act
            string result = wrappedText.UnwrapCSharp();

            // Assert
            result.Should().Be(expectedText);
        }

        [TestMethod]
        public void Test_IsCSharpClass_ValidClass_ReturnsTrue()
        {
            // Arrange
            string classText = @"using System;

            namespace MyNamespace
            {
                public class MyClass
                {
                    public int MyProperty { get; set; }
                }
            }
        ";

            // Act
            bool result = classText.IsCSharpCompilable();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Test_IsCSharpClass_InvalidClass_ReturnsFalse()
        {
            // Arrange
            string classText = @"using System;
            
            public class MyClass
            {
                public int MyProperty { get; set; }
        ";

            // Act
            bool result = classText.IsCSharpCompilable();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void Test_IsCSharpClass_ValidInterface_ReturnsTrue()
        {
            // Arrange
            string interfaceText = @"using System;

            namespace MyNamespace
            {
            public interface IMyInterface
            {
                void MyMethod();
            }
            }
        ";

            // Act
            bool result = interfaceText.IsCSharpCompilable();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Test_IsCSharpClass_ValidAbstract_ReturnsTrue()
        {
            // Arrange
            string abstractClassText = @"using System;
            namespace MyNamespace
            {
            public abstract class MyBaseClass
            {
                public abstract void MyMethod();
            }            }
        ";

            // Act
            bool result = abstractClassText.IsCSharpCompilable();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Test_IsCSharpClass_missing_bracket_at_end_ReturnsFalse()
        {
            // Arrange
            string abstractClassText = @"using System;
            namespace MyNamespace
            {
            public abstract class MyBaseClass
            {
                public abstract void MyMethod();
            }
        ";

            // Act
            bool result = abstractClassText.IsCSharpCompilable();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void Test_IsCSharpClass_no_using_ReturnsTrue()
        {
            // Arrange
            string codeBlockWithoutUsing = @"namespace MyNamespace
            {
                public class MyClass
                {
                    // Class members here
                }
            }
        ";

            string pattern = @"^(?:using.*?;)?\s*.*?\bnamespace\b.*?\b(class|interface|abstract|enum)\b.*?}\s*}\s*$";

            // Act
            bool isMatch = Regex.IsMatch(codeBlockWithoutUsing, pattern, RegexOptions.Singleline);

            // Assert
            Assert.IsTrue(isMatch, "The code block without using does not match the pattern.");
        }
    }
}