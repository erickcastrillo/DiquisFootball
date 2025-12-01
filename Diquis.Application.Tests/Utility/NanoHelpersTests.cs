using Diquis.Application.Utility;
using Diquis.Application.Common.Filter;
using System.ComponentModel;
using Xunit;
using System.Collections.Generic;

namespace Diquis.Application.Tests.Utility
{
    public class NanoHelpersTests
    {
        private enum TestEnumWithDescription
        {
            [Description("Test Description")]
            Value1
        }

        private enum TestEnumWithoutDescription
        {
            Value2
        }

        [Theory]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        public void GenerateHex_ReturnsCorrectLength(int digits)
        {
            // Act
            var result = NanoHelpers.GenerateHex(digits);

            // Assert
            Assert.Equal(digits, result.Length);
        }

        [Fact]
        public void GetEnumDescription_ReturnsDescription()
        {
            // Arrange
            var enumValue = TestEnumWithDescription.Value1;

            // Act
            var result = NanoHelpers.GetEnumDescription(enumValue);

            // Assert
            Assert.Equal("Test Description", result);
        }

        [Fact]
        public void GetEnumDescription_ReturnsEnumNameWhenNoDescription()
        {
            // Arrange
            var enumValue = TestEnumWithoutDescription.Value2;

            // Act
            var result = NanoHelpers.GetEnumDescription(enumValue);

            // Assert
            Assert.Equal("Value2", result);
        }

        [Fact]
        public void ReplaceWhitespace_ReplacesSpacesWithUnderscore()
        {
            // Arrange
            var input = "this is a test";

            // Act
            var result = NanoHelpers.ReplaceWhitespace(input, "_");

            // Assert
            Assert.Equal("this_is_a_test", result);
        }

        [Theory]
        [InlineData("This is a Test", "this-is-a-test")]
        [InlineData("   leading and trailing spaces   ", "leading-and-trailing-spaces")]
        [InlineData("Special---Characters!@#$%", "special-characters")]
        [InlineData("Accénted Chars éàç", "accented-chars-eac")]
        public void ToUrlSlug_ConvertsStringToSlug(string input, string expected)
        {
            // Act
            var result = NanoHelpers.ToUrlSlug(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void RemoveAccents_RemovesAccentsCorrectly()
        {
            // Arrange
            var input = "éàçüö";

            // Act
            var result = NanoHelpers.RemoveAccents(input);

            // Assert
            Assert.Equal("eacuo", result);
        }

        [Fact]
        public void GenerateOrderByString_WithSingleSort_GeneratesCorrectString()
        {
            // Arrange
            var filter = new PaginationFilter
            {
                Sorting = new List<TanstackColumnOrder> { new TanstackColumnOrder { Id = "Name", Desc = false } }
            };

            // Act
            var result = NanoHelpers.GenerateOrderByString(filter);

            // Assert
            Assert.Equal("Name", result);
        }

        [Fact]
        public void GenerateOrderByString_WithMultipleSorts_GeneratesCorrectString()
        {
            // Arrange
            var filter = new PaginationFilter
            {
                Sorting = new List<TanstackColumnOrder>
                {
                    new TanstackColumnOrder { Id = "Name", Desc = false },
                    new TanstackColumnOrder { Id = "Date", Desc = true }
                }
            };

            // Act
            var result = NanoHelpers.GenerateOrderByString(filter);

            // Assert
            Assert.Equal("Name,-Date", result);
        }

        [Fact]
        public void GenerateOrderByString_WithDescendingSort_GeneratesCorrectString()
        {
            // Arrange
            var filter = new PaginationFilter
            {
                Sorting = new List<TanstackColumnOrder> { new TanstackColumnOrder { Id = "Name", Desc = true } }
            };

            // Act
            var result = NanoHelpers.GenerateOrderByString(filter);

            // Assert
            Assert.Equal("-Name", result);
        }
    }
}
