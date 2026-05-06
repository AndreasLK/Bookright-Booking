using Domain.Value_Objects;
using Domain;
using Xunit;

namespace Tests.Domain.ValueObjects
{
        /// <summary>
        /// Validates that EmailAddress enforces domain rules.
        /// </summary>
        public class EmailAddressTests
        {
                [Fact]
                public void Constructor_WithValidEmail_ShouldInitialize()
                {
                        // Arrange
                        var validEmail = "contact@bookright.dk";

                        // Act
                        var email = new EmailAddress(value: validEmail);

                        // Assert
                        Assert.Equal(expected: validEmail, actual: email.Value);
                }

                [Theory]
                [InlineData("")]
                [InlineData("invalid.email.com")] // Missing @
                public void Constructor_WithInvalidEmail_ShouldThrowArgumentException(string emailInput)
                {
                        // Assert & Act
                        Assert.Throws<ArgumentException>(testCode: () => new EmailAddress(value: emailInput));
                }

                [Fact]
                public void Constructor_WithNull_ShouldThrowArgumentNullException()
                {
                        // Assert & Act
                        Assert.Throws<ArgumentNullException>(testCode: () => new EmailAddress(value: null!));
                }
        }
}
