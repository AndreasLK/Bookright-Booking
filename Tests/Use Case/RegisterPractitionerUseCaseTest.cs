using Domain.Entities;
using Domain.Entities.Persons;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Specifications.Customers;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using Moq;
using UseCase.Practitioners;
using Xunit;

namespace Tests.UseCase.Practitioners
{
        public class RegisterPractitionerUseCaseTests
        {
                // -----------------------------------------------------------------
                // Helper methods - build valid objects for the tests.
                // Each test starts from a "known good" baseline and only
                // changes the one thing it actually wants to test.
                // -----------------------------------------------------------------

                private static RegisterPractitionerCommand BuildValidCommand(
                    string email = "test@test.dk",
                    string phoneNumber = "12345678",
                    IReadOnlyList<Certificate>? certificates = null)
                {
                        return new RegisterPractitionerCommand(
                            LegalFirstName: "Test",
                            LegalLastName: "Practitioner",
                            Alias: "Doc Test",
                            Pronouns: "they/them",
                            DateOfBirth: new DateOnly(1985, 6, 15),
                            Email: email,
                            PhoneNumber: phoneNumber,
                            Gender: Gender.Abinary,
                            Certificates: certificates ?? new List<Certificate>());
                }

                private static Certificate BuildCertificate(
                    string name = "Test Certificate",
                    AuthorizationType type = AuthorizationType.Physiotherapy,
                    string? certificateId = null)
                {
                        return new Certificate(
                            name: name,
                            certificateId: new CertificateId(certificateId ?? Guid.NewGuid().ToString()),
                            authorizationType: type,
                            validFrom: new DateOnly(2020, 1, 1));
                }

                [Fact]

                public async Task ExecuteAsync_ShouldCreatePractitioner_whenCommandIsValid()
                {
                        var repo = new Mock<IPractitionerRepository>();
                        repo.Setup(p => p.AddAsync(It.IsAny<Practitioner>())).ReturnsAsync((Practitioner p) => p);

                        var useCase = new RegisterPractitionerUseCase(repo.Object);
                        var cmd = BuildValidCommand();

                        //ACT

                        var result = await useCase.ExecuteAsync(cmd);

                        Assert.True(result.Success);
                        Assert.NotNull(result.PractitionerId);
                        Assert.Null(result.ErrorMessage);
                        repo.Verify(r => r.AddAsync(It.IsAny<Practitioner>()), Times.Once);

                }

                [Fact]
                public async Task ExecuteAsync_ShouldReturnError_WhenEmailIsInvalid()
                {
                        // Arrange
                        var repo = new Mock<IPractitionerRepository>();
                        var useCase = new RegisterPractitionerUseCase(repo.Object);
                        var cmd = BuildValidCommand(email: "not-an-email");

                        // Act
                        var result = await useCase.ExecuteAsync(cmd);

                        // Assert
                        Assert.False(result.Success);
                        Assert.Null(result.PractitionerId);
                        Assert.NotNull(result.ErrorMessage);
                        repo.Verify(r => r.AddAsync(It.IsAny<Practitioner>()), Times.Never);
                }

                [Fact]

                public async Task ExecuteAsync_ShouldReturnError_WhenPhoneIsInvalid()
                {

                        //Arrange
                        var repo = new Mock<IPractitionerRepository>();
                        var useCase = new RegisterPractitionerUseCase(repo.Object);
                        var cmd = BuildValidCommand(phoneNumber: "");

                        //ACT

                        var result = await useCase.ExecuteAsync(cmd);

                        //Assert
                        Assert.False(result.Success);
                        Assert.Null(result.PractitionerId);
                        Assert.NotNull(result.ErrorMessage);
                        repo.Verify(r => r.AddAsync(It.IsAny<Practitioner>()), Times.Never);

                }

                [Fact]
                public async Task ExecuteAsync_ShouldReturnError_WhenDuplicateCertificate()
                {
                        var repo = new Mock<IPractitionerRepository>();
                        var cert1 = BuildCertificate(certificateId: "123");
                        var cert2 = BuildCertificate(certificateId: "123");

                        var useCase = new RegisterPractitionerUseCase(repo.Object);
                        var cmd = BuildValidCommand(certificates: new List<Certificate> { cert1, cert2 });

                        var result = await useCase.ExecuteAsync(cmd);

                        Assert.False(result.Success);
                        Assert.Null(result.PractitionerId);
                        Assert.NotNull(result.ErrorMessage);
                        repo.Verify(r => r.AddAsync(It.IsAny<Practitioner>()), Times.Never);


                }

                [Fact]
                public async Task ExecuteAsync_ShouldCreatePractitioner_WhenCertificatesListIsEmpty()
                {
                        // Arrange
                        var repo = new Mock<IPractitionerRepository>();
                        repo.Setup(r => r.AddAsync(It.IsAny<Practitioner>()))
                            .ReturnsAsync((Practitioner p) => p);

                        var useCase = new RegisterPractitionerUseCase(repo.Object);
                        var cmd = BuildValidCommand(
                            certificates: new List<Certificate>());   // explicitly empty

                        // Act
                        var result = await useCase.ExecuteAsync(cmd);

                        // Assert
                        Assert.True(result.Success);
                        Assert.NotNull(result.PractitionerId);
                        Assert.Null(result.ErrorMessage);
                        repo.Verify(r => r.AddAsync(It.IsAny<Practitioner>()), Times.Once);
                }


                // -----------------------------------------------------------------
                // Test 6: If the repository itself fails (e.g., database down),
                // the use case must catch the exception and return a clean error
                // result instead of propagating the raw exception to the caller.
                // -----------------------------------------------------------------

                [Fact]
                public async Task ExecuteAsync_ShouldReturnError_WhenRepositoryThrows()
                {
                        // Arrange
                        var repo = new Mock<IPractitionerRepository>();
                        repo.Setup(r => r.AddAsync(It.IsAny<Practitioner>()))
                            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

                        var useCase = new RegisterPractitionerUseCase(repo.Object);
                        var cmd = BuildValidCommand();

                        // Act
                        var result = await useCase.ExecuteAsync(cmd);

                        // Assert
                        Assert.False(result.Success);
                        Assert.Null(result.PractitionerId);
                        Assert.NotNull(result.ErrorMessage);
                        Assert.Contains("Database connection failed", result.ErrorMessage);
                        repo.Verify(r => r.AddAsync(It.IsAny<Practitioner>()), Times.Once);
                }


                // -----------------------------------------------------------------
                // Test 7: When multiple certificates are provided, ALL of them
                // must end up attached to the persisted practitioner - not just
                // the first or last one.
                // -----------------------------------------------------------------

                [Fact]
                public async Task ExecuteAsync_ShouldAddAllCertificates_WhenMultipleProvided()
                {
                        // Arrange
                        var repo = new Mock<IPractitionerRepository>();

                        Practitioner? savedPractitioner = null;
                        repo.Setup(r => r.AddAsync(It.IsAny<Practitioner>()))
                            .ReturnsAsync((Practitioner p) => p)
                            .Callback((Practitioner p) => savedPractitioner = p);

                        var useCase = new RegisterPractitionerUseCase(repo.Object);
                        var cmd = BuildValidCommand(certificates: new List<Certificate>
    {
                        BuildCertificate(certificateId: "FYS-001", type: AuthorizationType.Physiotherapy),
                        BuildCertificate(certificateId: "AKU-002", type: AuthorizationType.Acupuncture),
                        BuildCertificate(certificateId: "KOST-003", type: AuthorizationType.DietaryConsultation)
    });

                        // Act
                        var result = await useCase.ExecuteAsync(cmd);

                        // Assert
                        Assert.True(result.Success);
                        Assert.NotNull(savedPractitioner);
                        Assert.Equal(3, savedPractitioner!.Certificates.Count);
                        Assert.Contains(savedPractitioner.Certificates, c => c.CertificateId.Value == "FYS-001");
                        Assert.Contains(savedPractitioner.Certificates, c => c.CertificateId.Value == "AKU-002");
                        Assert.Contains(savedPractitioner.Certificates, c => c.CertificateId.Value == "KOST-003");
                }



                // TODO: Test 1 - happy path: ShouldCreatePractitioner_WhenCommandIsValid
                // TODO: Test 2 - unhappy:    ShouldReturnError_WhenEmailIsInvalid
                // TODO: Test 3 - unhappy:    ShouldReturnError_WhenPhoneIsInvalid
                // TODO: Test 4 - unhappy:    ShouldReturnError_WhenDuplicateCertificate
                // TODO: Test 5 - unhappy:    ShouldNotCallAddAsync_WhenValidationFails
                // TODO: Test 6 - unhappy:    ShouldReturnError_WhenRepositoryThrows
                // TODO: Test 7 - happy:      ShouldAddAllCertificates_WhenMultipleProvided
        }
}
