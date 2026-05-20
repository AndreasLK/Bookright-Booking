using Moq;
using UseCase.Customers;
using Domain.Interfaces.Repositories;
using Domain.Entities.Persons;
using Domain.Enums;
using Domain.Specifications.Customers;
using Xunit;

public class RegisterCustomerUseCaseTests
{
        [Fact]
        public async Task ExecuteAsync_ShouldReturnError_WhenEmailAlreadyExists()
        {
                // Arrange
                var repo = new Mock<ICustomerRepository>();

                // Mock: repository finds a excisting customer
                repo.Setup(r => r.FindAsync(It.IsAny<CustomerByEmailSpecification>()))
                        .ReturnsAsync(new List<Customer> { new Mock<Customer>().Object });

                var useCase = new RegisterCustomerUseCase(repo.Object);

                var cmd = new RegisterCustomerCommand(
                                "Test",                                    // LegalFirstName
                                "Person",                                  // LegalLastName
                                "he/him",                          // Pronouns
                                new DateOnly(1990, 1, 1),   // DateOfBirth
                                "12345678",                     // PhoneNumber
                                "test@test.dk",                      // Email
                                Gender.Abinary,                           // Gender
                                "note",                        // PersonalNote
                                "important"                   // ImportantNote
                        );
                

                // Act
                var result = await useCase.ExecuteAsync(cmd);

                // Assert
                Assert.False(result.Success);
                Assert.Equal("En kunde med denne email findes allerede.", result.ErrorMessage);
                repo.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
        }
}
