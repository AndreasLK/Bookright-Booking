using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Domain.Entities;
using Domain.Entities.Persons;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using Use_Case.BestDiscount;

namespace Tests.UseCase.BestDiscount
{
        public class DiscountContextFactoryTests
        {
                /// <summary>
                /// Verifies that instantiating the factory without a valid currency converter
                /// correctly throws an ArgumentNullException to prevent invalid state.
                /// </summary>
                [Fact]
                public void Constructor_WithNullCurrencyConverter_ThrowsArgumentNullException()
                {
                        // Arrange
                        var mockCampaignRepo = new Mock<ICampaignRepository>();
                        var mockBookingRepo = new Mock<IBookingRepository>();
                        var mockTreatmentRepo = new Mock<ITreatmentRepository>();
                        var mockCustomerRepo = new Mock<ICustomerRepository>();
                        ICurrencyConverter? nullCurrencyConverter = null;

                        // Act & Assert
                        var exception = Assert.Throws<ArgumentNullException>(() =>
                            new DiscountContextFactory(
                                mockCampaignRepo.Object,
                                mockBookingRepo.Object,
                                mockTreatmentRepo.Object,
                                mockCustomerRepo.Object,
                                nullCurrencyConverter!));

                        Assert.Equal("currencyConverter", exception.ParamName);
                }

                /// <summary>
                /// Ensures that if the booking ID provided does not exist in the database,
                /// the factory fails fast rather than attempting to process null data.
                /// </summary>
                [Fact]
                public async Task CreateAsync_WhenBookingIsNotFound_ThrowsArgumentNullException()
                {
                        // Arrange
                        var mockCampaignRepo = new Mock<ICampaignRepository>();
                        var mockBookingRepo = new Mock<IBookingRepository>();
                        var mockTreatmentRepo = new Mock<ITreatmentRepository>();
                        var mockCustomerRepo = new Mock<ICustomerRepository>();
                        var mockCurrencyConverter = new Mock<ICurrencyConverter>();

                        var systemUnderTest = new DiscountContextFactory(
                                mockCampaignRepo.Object,
                                mockBookingRepo.Object,
                                mockTreatmentRepo.Object,
                                mockCustomerRepo.Object,
                                mockCurrencyConverter.Object);

                        var targetCustomerId = new CustomerId(Guid.NewGuid());
                        var targetBookingId = new BookingId(Guid.NewGuid());

                        mockCampaignRepo.Setup(repo => repo.GetActiveAsync()).ReturnsAsync(new List<Campaign>());

                        // Simulating a missing booking
                        mockBookingRepo.Setup(repo => repo.GetByIdAsync(targetBookingId.Value)).ReturnsAsync((Booking?)null);

                        // Act & Assert
                        await Assert.ThrowsAsync<ArgumentNullException>(() =>
                            systemUnderTest.CreateAsync(targetCustomerId, targetBookingId));
                }

                /// <summary>
                /// Verifies that the factory correctly loops through past bookings, converts mixed
                /// currencies into a single standardized DKK total, and maps the usage dictionary correctly.
                /// </summary>
                [Fact]
                public async Task CreateAsync_WithMixedCurrencyHistoricalBookings_CorrectlyAggregatesTotalSpendInDkk()
                {
                        // Arrange
                        var mockCampaignRepo = new Mock<ICampaignRepository>();
                        var mockBookingRepo = new Mock<IBookingRepository>();
                        var mockTreatmentRepo = new Mock<ITreatmentRepository>();
                        var mockCustomerRepo = new Mock<ICustomerRepository>();
                        var mockCurrencyConverter = new Mock<ICurrencyConverter>();

                        var systemUnderTest = new DiscountContextFactory(
                                mockCampaignRepo.Object,
                                mockBookingRepo.Object,
                                mockTreatmentRepo.Object,
                                mockCustomerRepo.Object,
                                mockCurrencyConverter.Object);

                        var customerId = new CustomerId(Guid.NewGuid());
                        var bookingId = new BookingId(Guid.NewGuid());
                        var treatmentId = new TreatmentId(Guid.NewGuid());
                        var campaignId = new CampaignId(Guid.NewGuid());

                        // 1. Instantiate the mocks using the exact constructor mapping below
                        var mockTreatment = CreateTestTreatment(treatmentId, new Money(500m, Currency.DKK));
                        var mockBooking = CreateTestBooking(treatmentId, new TimeSlot(DateTime.UtcNow, DateTime.UtcNow.AddHours(1)), null, null);
                        var mockCustomer = CreateTestCustomer(customerId, new DateOnly(1990, 5, 15));

                        mockCampaignRepo.Setup(repo => repo.GetActiveAsync()).ReturnsAsync(new List<Campaign>());
                        mockBookingRepo.Setup(repo => repo.GetByIdAsync(bookingId.Value)).ReturnsAsync(mockBooking);
                        mockTreatmentRepo.Setup(repo => repo.GetByIdAsync(treatmentId.Value)).ReturnsAsync(mockTreatment);
                        mockCustomerRepo.Setup(repo => repo.GetByIdAsync(customerId.Value)).ReturnsAsync(mockCustomer);

                        // 2. Setup Historical Bookings using exact constructors
                        var historicalBookingOne = CreateTestBooking(
                                treatmentId,
                                new TimeSlot(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-10).AddHours(1)),
                                new Money(10m, Currency.EUR),
                                campaignId);

                        var historicalBookingTwo = CreateTestBooking(
                                treatmentId,
                                new TimeSlot(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(-5).AddHours(1)),
                                new Money(50m, Currency.DKK),
                                null); // No campaign used for this one

                        var historicalBookings = new List<Booking> { historicalBookingOne, historicalBookingTwo };

                        mockBookingRepo.Setup(repo => repo.FindAsync(It.IsAny<PaidBookingsByCustomerSpecification>()))
                                       .ReturnsAsync(historicalBookings);

                        // Setup Currency Converter logic specific to this test
                        mockCurrencyConverter.Setup(converter => converter.Convert(10m, Currency.EUR, Currency.DKK))
                                             .Returns(new Money(74.60m, Currency.DKK));
                        mockCurrencyConverter.Setup(converter => converter.Convert(50m, Currency.DKK, Currency.DKK))
                                             .Returns(new Money(50m, Currency.DKK));

                        // Act
                        DiscountContext resultContext = await systemUnderTest.CreateAsync(customerId, bookingId);

                        // Assert
                        decimal expectedTotalSpend = 74.60m + 50m; // 124.60 DKK

                        Assert.Equal(expectedTotalSpend, resultContext.TotalHistoricalSpend.Value);
                        Assert.Equal(Currency.DKK, resultContext.TotalHistoricalSpend.Currency);
                        Assert.Equal(Month.May, resultContext.CustomerBirthMonth);
                        Assert.True(resultContext.TimeUsedEligbleCampaigns.ContainsKey(campaignId));
                        Assert.Single(resultContext.TimeUsedEligbleCampaigns[campaignId]);
                }

                // --- Helper Methods to adhere to strict DDD Rules ---

                private static Booking CreateTestBooking(TreatmentId treatmentId, TimeSlot timeSlot, Money? paid, CampaignId? appliedCampaign)
                {
                        // Perfectly maps to your actual Booking.cs constructor signature
                        return new Booking(
                                id: new BookingId(Guid.NewGuid()),
                                clinic: new ClinicId(Guid.NewGuid()),
                                practitioner: new PractitionerId(Guid.NewGuid()),
                                treatment: treatmentId,
                                room: new RoomId(Guid.NewGuid()),
                                customer: new CustomerId(Guid.NewGuid()),
                                timeslot: timeSlot,
                                createdAt: DateTime.UtcNow,
                                paid: paid,
                                appliedCampaign: appliedCampaign
                        );
                }

                private static Treatment CreateTestTreatment(TreatmentId id, Money price)
                {
                        // Target the compiler-generated backing fields directly to bypass 'private set' restrictions
                        var treatment = (Treatment)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Treatment));

                        typeof(Treatment).GetField("<Id>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                            ?.SetValue(treatment, id);

                        typeof(Treatment).GetField("<Price>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                            ?.SetValue(treatment, price);

                        return treatment;
                }

                private static Customer CreateTestCustomer(CustomerId id, DateOnly dateOfBirth)
                {
                        // 1. Create dummy value objects safely to bypass any strict validation they might have
                        var dummyPhone = (PhoneNumber)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(PhoneNumber));
                        var dummyEmail = (EmailAddress)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(EmailAddress));

                        // 2. Instantiate a REAL PersonDetails object with valid strings to satisfy the Person.ctor guards!
                        var validDetails = new PersonDetails(
                            LegalFirstName: "John",
                            LegalLastName: "Doe",
                            Pronouns: "He/Him",
                            DateOfBirth: dateOfBirth,
                            PhoneNumber: dummyPhone,
                            Email: dummyEmail,
                            Gender: default(Gender) // Safely grabs the first value of your Gender enum
                        );

                        // 3. Instantiate the Customer safely using its ACTUAL constructor
                        return new Customer(
                            id: id,
                            personalNote: "Test Note",
                            importantNote: null,
                            preferredPratitionerId: null,
                            preferredGender: null,
                            sygsikringDanmarkMember: false,
                            details: validDetails
                        );
                }
        }
}
