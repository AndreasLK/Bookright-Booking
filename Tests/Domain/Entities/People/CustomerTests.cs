using Domain.Entities.Persons;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using Domain.Enums;
using Xunit;

namespace Tests.Domain.Entities
{
        /// <summary>
        /// Internal test suite for the <see cref="Customer"/> domain entity.
        /// Covers property mapping, inheritance, and guard clauses from the base Person class.
        /// </summary>
        public class CustomerTests
        {
                /// <summary>
                /// Helper to create valid PersonDetails, keeping test methods flat and readable.
                /// </summary>
                private PersonDetails CreateValidTestDetails(
                        string firstName = "Jane",
                        string lastName = "Doe",
                        string pronouns = "She/Her")
                {
                        return new PersonDetails(
                                LegalFirstName: firstName,
                                LegalLastName: lastName,
                                Pronouns: pronouns,
                                DateOfBirth: new DateOnly(year: 1990, month: 5, day: 20),
                                PhoneNumber: new PhoneNumber(value: "12345678"),
                                Email: new EmailAddress(value: "jane@bookright.dk"),
                                Gender: Gender.Woman);
                }

                [Fact]
                public void Constructor_WithValidData_ShouldMapAllPropertiesCorrectly()
                {
                        // Arrange
                        var customerId = new CustomerId(Value: Guid.NewGuid());
                        var practitionerId = Guid.NewGuid();
                        var details = this.CreateValidTestDetails();

                        // Act
                        var customer = new Customer(
                                id: customerId,
                                personalNote: "Note A",
                                importantNote: "Note B",
                                preferredPratitionerId: practitionerId,
                                preferredGender: Gender.NonBinary,
                                sygsikringDanmarkMember: true,
                                details: details);

                        // Assert - Customer specific
                        Assert.Equal(expected: customerId, actual: customer.Id);
                        Assert.Equal(expected: "Note A", actual: customer.PersonalNote);
                        Assert.Equal(expected: practitionerId, actual: customer.PreferredPratitionerId);

                        // Assert - Inherited from Person
                        Assert.Equal(expected: details.LegalFirstName, actual: customer.LegalFirstName);
                        Assert.Equal(expected: details.Email, actual: customer.Email);
                }

                [Fact]
                public void Constructor_ShouldThrowArgumentNullException_WhenIdIsNull()
                {
                        // Assert & Act
                        Assert.Throws<ArgumentNullException>(testCode: () => new Customer(
                                id: null!,
                                personalNote: null,
                                importantNote: null,
                                preferredPratitionerId: null,
                                preferredGender: null,
                                sygsikringDanmarkMember: false,
                                details: this.CreateValidTestDetails()));
                }

                [Fact]
                public void Constructor_ShouldThrowArgumentNullException_WhenDetailsIsNull()
                {
                        // Assert & Act
                        Assert.Throws<ArgumentNullException>(testCode: () => new Customer(
                                id: new CustomerId(Value: Guid.NewGuid()),
                                personalNote: null,
                                importantNote: null,
                                preferredPratitionerId: null,
                                preferredGender: null,
                                sygsikringDanmarkMember: false,
                                details: null!));
                }

                [Theory]
                [InlineData("")]
                [InlineData(" ")]
                [InlineData(null)]
                public void Constructor_ShouldThrowArgumentException_WhenLegalFirstNameIsInvalid(string invalidName)
                {
                        // Arrange
                        var details = this.CreateValidTestDetails(firstName: invalidName);

                        // Assert & Act
                        Assert.Throws<ArgumentException>(testCode: () => new Customer(
                                id: new CustomerId(Value: Guid.NewGuid()),
                                personalNote: null,
                                importantNote: null,
                                preferredPratitionerId: null,
                                preferredGender: null,
                                sygsikringDanmarkMember: false,
                                details: details));
                }

                [Fact]
                public void UnwantedGenders_ShouldBeInitializedEmpty()
                {
                        // Arrange & Act
                        var customer = new Customer(
                                id: new CustomerId(Value: Guid.NewGuid()),
                                personalNote: null,
                                importantNote: null,
                                preferredPratitionerId: null,
                                preferredGender: null,
                                sygsikringDanmarkMember: false,
                                details: this.CreateValidTestDetails());

                        // Assert
                        Assert.Empty(collection: customer.UnwantedGenders);
                }

                [Fact]
                public void Loyality_ShouldDefaultToUnspecified()
                {
                        // Arrange & Act
                        var customer = new Customer(
                                id: new CustomerId(Value: Guid.NewGuid()),
                                personalNote: null,
                                importantNote: null,
                                preferredPratitionerId: null,
                                preferredGender: null,
                                sygsikringDanmarkMember: false,
                                details: this.CreateValidTestDetails());

                        // Assert
                        Assert.Equal(expected: default(LoyalityLevel), actual: customer.Loyality);
                }
        }
}
