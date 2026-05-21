using Moq;
using UseCase.Customers;
using Domain.Interfaces.Repositories;
using Domain.Entities.Persons;
using Domain.Enums;
using Domain.Specifications.Customers;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using Xunit;

namespace Tests.UseCase.Customers
{
        public class RegisterCustomerUseCaseTests
        {
                // -----------------------------------------------------------------
                // Helper methods - build valid objects for the tests.
                // We place them here so each test can start from a "known good"
                // baseline and only change the one thing it actually wants to test.
                // -----------------------------------------------------------------

                private static RegisterCustomerCommand BuildValidCommand(string email = "test@test.dk")
                {
                        return new RegisterCustomerCommand(
                                LegalFirstName: "Test",
                                LegalLastName: "Person",
                                Pronouns: "he/him",
                                DateOfBirth: new DateOnly(1990, 1, 1),
                                PhoneNumber: "12345678",
                                Email: email,
                                Gender: Gender.Abinary,
                                PersonalNote: "note",
                                ImportantNote: "important");
                }

                private static Customer BuildExistingCustomer(string email = "test@test.dk")
                {
                        // We build a REAL Customer instance. This is much simpler than
                        // trying to mock the entity - and it's exactly how an entity
                        // would look if it came back from the repository.
                        var details = new PersonDetails(
                                LegalFirstName: "Existing",
                                LegalLastName: "Customer",
                                Pronouns: "they/them",
                                DateOfBirth: new DateOnly(1985, 5, 5),
                                PhoneNumber: new PhoneNumber("87654321"),
                                Email: new EmailAddress(email),
                                Gender: Gender.Abinary);

                        return new Customer(
                                id: new CustomerId(Guid.NewGuid()),
                                personalNote: null,
                                importantNote: null,
                                preferredPratitionerId: null,
                                preferredGender: null,
                                sygsikringDanmarkMember: false,
                                details: details);
                }

                // -----------------------------------------------------------------
                // Test 1: When a customer with the same email already exists,
                // the use case must return an error AND must not call AddAsync.
                // -----------------------------------------------------------------

                [Fact]
                public async Task ExecuteAsync_ShouldReturnError_WhenEmailAlreadyExists()
                {
                        // Arrange
                        var repo = new Mock<ICustomerRepository>();

                        // Set FindAsync up to return a list containing one existing customer.
                        // Note: the return type on the interface is IReadOnlyList<Customer>, so we cast.
                        IReadOnlyList<Customer> existing = new List<Customer> { BuildExistingCustomer() };
                        repo.Setup(r => r.FindAsync(It.IsAny<CustomerByEmailSpecification>()))
                                .ReturnsAsync(existing);

                        var useCase = new RegisterCustomerUseCase(repo.Object);
                        var cmd = BuildValidCommand();

                        // Act
                        var result = await useCase.ExecuteAsync(cmd);

                        // Assert
                        Assert.False(result.Success);
                        Assert.Null(result.CustomerId);
                        Assert.Equal("En kunde med denne email findes allerede.", result.ErrorMessage);
                        // Important: we must NOT have attempted to persist a new customer.
                        repo.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
                }

                // -----------------------------------------------------------------
                // Test 2: When NO customer with the same email exists,
                // the use case must create the customer and call AddAsync once.
                // -----------------------------------------------------------------

                [Fact]
                public async Task ExecuteAsync_ShouldCreateCustomer_WhenEmailIsUnique()
                {
                        // Arrange
                        var repo = new Mock<ICustomerRepository>();

                        // Empty list = no existing customer with that email
                        IReadOnlyList<Customer> empty = new List<Customer>();
                        repo.Setup(r => r.FindAsync(It.IsAny<CustomerByEmailSpecification>()))
                                .ReturnsAsync(empty);

                        // AddAsync must return the incoming customer (per the interface contract)
                        repo.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                                .ReturnsAsync((Customer c) => c);

                        var useCase = new RegisterCustomerUseCase(repo.Object);
                        var cmd = BuildValidCommand(email: "new@test.dk");

                        // Act
                        var result = await useCase.ExecuteAsync(cmd);

                        // Assert
                        Assert.True(result.Success);
                        Assert.NotNull(result.CustomerId);
                        Assert.Null(result.ErrorMessage);
                        repo.Verify(r => r.AddAsync(It.Is<Customer>(c => c.Email.Value == "new@test.dk")),
                                Times.Once);
                }

                // -----------------------------------------------------------------
                // Test 3: If the command contains invalid data (here: empty first name),
                // the exception must be caught by the use case and returned as an error.
                // -----------------------------------------------------------------

                [Fact]
                public async Task ExecuteAsync_ShouldReturnError_WhenDomainValidationFails()
                {
                        // Arrange
                        var repo = new Mock<ICustomerRepository>();
                        IReadOnlyList<Customer> empty = new List<Customer>();
                        repo.Setup(r => r.FindAsync(It.IsAny<CustomerByEmailSpecification>()))
                                .ReturnsAsync(empty);

                        var useCase = new RegisterCustomerUseCase(repo.Object);

                        // Empty first name => Person ctor throws ArgumentException
                        var invalidCmd = new RegisterCustomerCommand(
                                LegalFirstName: "",
                                LegalLastName: "Person",
                                Pronouns: "he/him",
                                DateOfBirth: new DateOnly(1990, 1, 1),
                                PhoneNumber: "12345678",
                                Email: "test@test.dk",
                                Gender: Gender.Abinary,
                                PersonalNote: null,
                                ImportantNote: null);

                        // Act
                        var result = await useCase.ExecuteAsync(invalidCmd);

                        // Assert
                        Assert.False(result.Success);
                        Assert.Null(result.CustomerId);
                        Assert.NotNull(result.ErrorMessage);
                        repo.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
                }
        }
}
