using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Moq.Protected;
using Xunit;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Strategies;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using Use_Case.BestDiscount;

namespace Tests.UseCase.BestDiscount
{
        public class DiscountServiceTests
        {
                /// <summary>
                /// Verifies that instantiating the DiscountService without a valid booking repository
                /// correctly throws an ArgumentNullException to prevent invalid state.
                /// </summary>
                [Fact]
                public void Constructor_WithNullBookingRepository_ThrowsArgumentNullException()
                {
                        // Arrange
                        IBookingRepository? nullBookingRepository = null;
                        var mockTreatmentRepository = new Mock<ITreatmentRepository>();
                        var mockCurrencyConverter = new Mock<ICurrencyConverter>();

                        // Act & Assert
                        var exception = Assert.Throws<ArgumentNullException>(() =>
                            new DiscountService(nullBookingRepository!, mockTreatmentRepository.Object, mockCurrencyConverter.Object));

                        Assert.Equal("bookingRepository", exception.ParamName);
                }

                /// <summary>
                /// Verifies that instantiating the DiscountService without a valid treatment repository
                /// correctly throws an ArgumentNullException to prevent invalid state.
                /// </summary>
                [Fact]
                public void Constructor_WithNullTreatmentRepository_ThrowsArgumentNullException()
                {
                        // Arrange
                        var mockBookingRepository = new Mock<IBookingRepository>();
                        ITreatmentRepository? nullTreatmentRepository = null;
                        var mockCurrencyConverter = new Mock<ICurrencyConverter>();

                        // Act & Assert
                        var exception = Assert.Throws<ArgumentNullException>(() =>
                            new DiscountService(mockBookingRepository.Object, nullTreatmentRepository!, mockCurrencyConverter.Object));

                        Assert.Equal("treatmentRepository", exception.ParamName);
                }

                /// <summary>
                /// Verifies that instantiating the DiscountService without a valid currency converter
                /// correctly throws an ArgumentNullException to prevent invalid state.
                /// </summary>
                [Fact]
                public void Constructor_WithNullCurrencyConverter_ThrowsArgumentNullException()
                {
                        // Arrange
                        var mockBookingRepository = new Mock<IBookingRepository>();
                        var mockTreatmentRepository = new Mock<ITreatmentRepository>();
                        ICurrencyConverter? nullCurrencyConverter = null;

                        // Act & Assert
                        var exception = Assert.Throws<ArgumentNullException>(() =>
                            new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object, nullCurrencyConverter!));

                        Assert.Equal("currencyConverter", exception.ParamName);
                }

                /// <summary>
                /// Ensures that the GetBestDiscount method fails fast and throws an ArgumentNullException
                /// if it is provided with a null DiscountContext.
                /// </summary>
                [Fact]
                public void GetBestDiscount_WithNullContext_ThrowsArgumentNullException()
                {
                        // Arrange
                        var mockBookingRepository = new Mock<IBookingRepository>();
                        var mockTreatmentRepository = new Mock<ITreatmentRepository>();
                        var mockCurrencyConverter = new Mock<ICurrencyConverter>();
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object, mockCurrencyConverter.Object);

                        DiscountContext? nullDiscountContext = null;

                        // Act & Assert
                        var exception = Assert.Throws<ArgumentNullException>(() =>
                            systemUnderTest.GetBestDiscount(nullDiscountContext!));

                        Assert.Equal("context", exception.ParamName);
                }

                /// <summary>
                /// Tests that if a customer is not eligible for any active campaigns,
                /// the service simply returns the standard base price of the treatment.
                /// </summary>
                [Fact]
                public void GetBestDiscount_WithNoActiveCampaigns_ReturnsBasePrice()
                {
                        // Arrange
                        var mockBookingRepository = new Mock<IBookingRepository>();
                        var mockTreatmentRepository = new Mock<ITreatmentRepository>();
                        var mockCurrencyConverter = SetupMockCurrencyConverter();
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object, mockCurrencyConverter.Object);

                        var basePrice = new Money(100m, Currency.DKK);

                        var discountContext = new DiscountContext(
                            basePrice,
                            new TreatmentId(Guid.NewGuid()),
                            new Money(0m, Currency.DKK),
                            new List<Campaign>(),
                            new Dictionary<CampaignId, List<DateTime>>(),
                            Month.January
                        );

                        // Act
                        var resultPrice = systemUnderTest.GetBestDiscount(discountContext);

                        // Assert
                        Assert.Equal(basePrice.Value, resultPrice.Value);
                        Assert.Equal(basePrice.Currency, resultPrice.Currency);
                }

                /// <summary>
                /// Verifies that when multiple campaigns are active and valid, the service 
                /// correctly compares them and returns the absolute lowest price for the customer.
                /// </summary>
                [Fact]
                public void GetBestDiscount_WithMultipleCampaigns_ReturnsLowestPrice()
                {
                        // Arrange
                        var mockBookingRepository = new Mock<IBookingRepository>();
                        var mockTreatmentRepository = new Mock<ITreatmentRepository>();
                        var mockCurrencyConverter = SetupMockCurrencyConverter();
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object, mockCurrencyConverter.Object);

                        var basePrice = new Money(200m, Currency.DKK);
                        var expectedLowestPrice = new Money(150m, Currency.DKK);

                        var mockStrategyOne = SetupMockDiscountStrategy(mockCurrencyConverter, new Money(180m, Currency.DKK));
                        var mockStrategyTwo = SetupMockDiscountStrategy(mockCurrencyConverter, expectedLowestPrice);

                        var campaignOne = CreateTestCampaign(mockStrategyOne.Object);
                        var campaignTwo = CreateTestCampaign(mockStrategyTwo.Object);

                        var discountContext = new DiscountContext(
                            basePrice,
                            new TreatmentId(Guid.NewGuid()),
                            new Money(500m, Currency.DKK),
                            new List<Campaign> { campaignOne, campaignTwo },
                            new Dictionary<CampaignId, List<DateTime>>(),
                            Month.June
                        );

                        // Act
                        var resultPrice = systemUnderTest.GetBestDiscount(discountContext);

                        // Assert
                        Assert.Equal(expectedLowestPrice.Value, resultPrice.Value);
                        Assert.Equal(expectedLowestPrice.Currency, resultPrice.Currency);
                }

                /// <summary>
                /// Ensures that if a calculated discount somehow results in a price higher than 
                /// the normal base price, the system protects the customer by defaulting to the base price.
                /// </summary>
                [Fact]
                public void GetBestDiscount_WhenDiscountIsWorseThanBasePrice_ReturnsBasePrice()
                {
                        // Arrange
                        var mockBookingRepository = new Mock<IBookingRepository>();
                        var mockTreatmentRepository = new Mock<ITreatmentRepository>();
                        var mockCurrencyConverter = SetupMockCurrencyConverter();
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object, mockCurrencyConverter.Object);

                        var basePrice = new Money(100m, Currency.DKK);
                        var worseDiscountPrice = new Money(150m, Currency.DKK);

                        var mockStrategy = SetupMockDiscountStrategy(mockCurrencyConverter, worseDiscountPrice);
                        var activeCampaign = CreateTestCampaign(mockStrategy.Object);

                        var discountContext = new DiscountContext(
                            basePrice,
                            new TreatmentId(Guid.NewGuid()),
                            new Money(200m, Currency.DKK),
                            new List<Campaign> { activeCampaign },
                            new Dictionary<CampaignId, List<DateTime>>(),
                            Month.August
                        );

                        // Act
                        var resultPrice = systemUnderTest.GetBestDiscount(discountContext);

                        // Assert
                        Assert.Equal(basePrice.Value, resultPrice.Value);
                        Assert.Equal(basePrice.Currency, resultPrice.Currency);
                }

                /// <summary>
                /// Validates that if the customer has used a campaign before, their specific 
                /// usage dates are properly retrieved from the dictionary and passed to the strategy.
                /// </summary>
                [Fact]
                public void GetBestDiscount_WhenUsageHistoryExists_PassesCorrectUsageToStrategy()
                {
                        // Arrange
                        var mockBookingRepository = new Mock<IBookingRepository>();
                        var mockTreatmentRepository = new Mock<ITreatmentRepository>();
                        var mockCurrencyConverter = SetupMockCurrencyConverter();
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object, mockCurrencyConverter.Object);

                        var mockStrategy = SetupMockDiscountStrategy(mockCurrencyConverter, new Money(80m, Currency.DKK));

                        var targetCampaignId = new CampaignId(Guid.NewGuid());
                        var expectedUsageHistory = new List<DateTime> { new DateTime(2023, 10, 01) };

                        var activeCampaign = CreateTestCampaign(mockStrategy.Object, targetCampaignId);

                        var usageDictionary = new Dictionary<CampaignId, List<DateTime>>
                        {
                            { targetCampaignId, expectedUsageHistory }
                        };

                        var discountContext = new DiscountContext(
                            new Money(100m, Currency.DKK),
                            new TreatmentId(Guid.NewGuid()),
                            new Money(1000m, Currency.DKK),
                            new List<Campaign> { activeCampaign },
                            usageDictionary,
                            Month.December
                        );

                        // Act
                        systemUnderTest.GetBestDiscount(discountContext);

                        // Assert
                        mockStrategy.Protected().Verify(
                            "CalculatePrice",
                            Times.Once(),
                            ItExpr.IsAny<Money>(),
                            ItExpr.IsAny<Money>(),
                            ItExpr.IsAny<TreatmentId>(),
                            ItExpr.IsAny<Month?>(),
                            ItExpr.Is<List<DateTime>>(list => list == expectedUsageHistory)
                        );
                }

                /// <summary>
                /// Validates that if a customer has never used an active campaign before, 
                /// an empty list is instantiated and safely passed to the calculation strategy.
                /// </summary>
                [Fact]
                public void GetBestDiscount_WhenNoUsageHistoryExists_PassesEmptyListToStrategy()
                {
                        // Arrange
                        var mockBookingRepository = new Mock<IBookingRepository>();
                        var mockTreatmentRepository = new Mock<ITreatmentRepository>();
                        var mockCurrencyConverter = SetupMockCurrencyConverter();
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object, mockCurrencyConverter.Object);

                        var mockStrategy = SetupMockDiscountStrategy(mockCurrencyConverter, new Money(50m, Currency.DKK));

                        var activeCampaign = CreateTestCampaign(mockStrategy.Object);

                        var discountContext = new DiscountContext(
                            new Money(100m, Currency.DKK),
                            new TreatmentId(Guid.NewGuid()),
                            new Money(0m, Currency.DKK),
                            new List<Campaign> { activeCampaign },
                            new Dictionary<CampaignId, List<DateTime>>(), // Empty dictionary
                            null
                        );

                        // Act
                        systemUnderTest.GetBestDiscount(discountContext);

                        // Assert
                        mockStrategy.Protected().Verify(
                            "CalculatePrice",
                            Times.Once(),
                            ItExpr.IsAny<Money>(),
                            ItExpr.IsAny<Money>(),
                            ItExpr.IsAny<TreatmentId>(),
                            ItExpr.IsAny<Month?>(),
                            ItExpr.Is<List<DateTime>>(list => list != null && list.Count == 0)
                        );
                }

                /// <summary>
                /// EXPOSES A BUG: Verifies that if different campaigns return different currencies, 
                /// the service accurately converts them to compare their real-world value, 
                /// rather than just comparing the raw decimal numbers.
                /// </summary>
                [Fact]
                public void GetBestDiscount_WithMixedCurrencies_FailsToIdentifyTrueLowestPrice()
                {
                        // Arrange
                        var mockBookingRepository = new Mock<IBookingRepository>();
                        var mockTreatmentRepository = new Mock<ITreatmentRepository>();
                        var mockCurrencyConverter = SetupMockCurrencyConverter();
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object, mockCurrencyConverter.Object);

                        var basePrice = new Money(200m, Currency.DKK);

                        // Strategy 1 returns 10 EUR (Which is equal to 74.60 DKK)
                        var eurPrice = new Money(10m, Currency.EUR);
                        var mockStrategyEur = SetupMockDiscountStrategy(mockCurrencyConverter, eurPrice);

                        // Strategy 2 returns 50 DKK (This is the TRUE lowest price)
                        var dkkPrice = new Money(50m, Currency.DKK);
                        var mockStrategyDkk = SetupMockDiscountStrategy(mockCurrencyConverter, dkkPrice);

                        var campaignEur = CreateTestCampaign(mockStrategyEur.Object);
                        var campaignDkk = CreateTestCampaign(mockStrategyDkk.Object);

                        var discountContext = new DiscountContext(
                            basePrice,
                            new TreatmentId(Guid.NewGuid()),
                            new Money(0m, Currency.DKK),
                            new List<Campaign> { campaignEur, campaignDkk },
                            new Dictionary<CampaignId, List<DateTime>>(),
                            null
                        );

                        // Act
                        var resultPrice = systemUnderTest.GetBestDiscount(discountContext);

                        // Assert
                        Assert.Equal(dkkPrice.Value, resultPrice.Value);
                        Assert.Equal(dkkPrice.Currency, resultPrice.Currency);
                }

                /// <summary>
                /// EXPOSES A BUG: Verifies that a base price is correctly evaluated against a 
                /// discounted price of a different currency.
                /// </summary>
                [Fact]
                public void GetBestDiscount_WhenDiscountIsWorseButInDifferentCurrency_DefaultsToBasePrice()
                {
                        // Arrange
                        var mockBookingRepository = new Mock<IBookingRepository>();
                        var mockTreatmentRepository = new Mock<ITreatmentRepository>();
                        var mockCurrencyConverter = SetupMockCurrencyConverter();
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object, mockCurrencyConverter.Object);

                        // Base price is 50 DKK
                        var basePrice = new Money(50m, Currency.DKK);

                        // Discount is 10 EUR (74.60 DKK). This is WORSE than the base price.
                        var worseEurPrice = new Money(10m, Currency.EUR);
                        var mockStrategyEur = SetupMockDiscountStrategy(mockCurrencyConverter, worseEurPrice);

                        var activeCampaign = CreateTestCampaign(mockStrategyEur.Object);

                        var discountContext = new DiscountContext(
                            basePrice,
                            new TreatmentId(Guid.NewGuid()),
                            new Money(0m, Currency.DKK),
                            new List<Campaign> { activeCampaign },
                            new Dictionary<CampaignId, List<DateTime>>(),
                            null
                        );

                        // Act
                        var resultPrice = systemUnderTest.GetBestDiscount(discountContext);

                        // Assert
                        Assert.Equal(basePrice.Value, resultPrice.Value);
                        Assert.Equal(basePrice.Currency, resultPrice.Currency);
                }

                /// <summary>
                /// Proves that the DiscountContext strictly guards its borders. It is mathematically 
                /// impossible to create a DiscountContext if the ActiveCampaigns list is null.
                /// </summary>
                [Fact]
                public void DiscountContext_WithNullActiveCampaignsList_ThrowsArgumentNullException()
                {
                        // Act & Assert
                        var exception = Assert.Throws<ArgumentNullException>(() =>
                            new DiscountContext(
                                new Money(100m, Currency.DKK),
                                new TreatmentId(Guid.NewGuid()),
                                new Money(0m, Currency.DKK),
                                null!, // Forcing null to trigger the guard clause
                                new Dictionary<CampaignId, List<DateTime>>(),
                                null
                            ));

                        Assert.Equal("activeCampaigns", exception.ParamName);
                }

                private static Mock<ICurrencyConverter> SetupMockCurrencyConverter()
                {
                        var mockCurrencyConverter = new Mock<ICurrencyConverter>();

                        // 1. Set up the primary Convert method to simulate the exact real business logic
                        mockCurrencyConverter
                            .Setup(converter => converter.Convert(It.IsAny<decimal>(), It.IsAny<Currency>(), It.IsAny<Currency>()))
                            .Returns((decimal amount, Currency fromCurrency, Currency toCurrency) =>
                            {
                                    if (fromCurrency == toCurrency)
                                    {
                                            return new Money(amount, fromCurrency);
                                    }
                                    else if (fromCurrency == Currency.DKK && toCurrency == Currency.EUR)
                                    {
                                            return new Money(amount * 0.134m, toCurrency);
                                    }
                                    else if (fromCurrency == Currency.EUR && toCurrency == Currency.DKK)
                                    {
                                            return new Money(amount * 7.46m, toCurrency);
                                    }
                                    else
                                    {
                                            return new Money(amount, fromCurrency);
                                    }
                            });

                        // 2. Set up the new overload (Money, Currency) to route to the primary method
                        mockCurrencyConverter
                            .Setup(converter => converter.Convert(It.IsAny<Money>(), It.IsAny<Currency>()))
                            .Returns((Money money, Currency targetCurrency) =>
                            {
                                    if (money is null) throw new ArgumentNullException(nameof(money));

                                    // Route the call through the primary mocked method above
                                    return mockCurrencyConverter.Object.Convert(money.Value, money.Currency, targetCurrency);
                            });

                        // 3. Set up the array conversion method
                        mockCurrencyConverter
                            .Setup(converter => converter.ConvertToSame(It.IsAny<Money[]>(), It.IsAny<Currency>()))
                            .Returns((Money[] values, Currency targetCurrency) =>
                            {
                                    if (values is null || values.Length == 0)
                                    {
                                            return Array.Empty<Money>();
                                    }

                                    // Route each item through the overloaded mock method
                                    return values
                                        .Select(money => mockCurrencyConverter.Object.Convert(money, targetCurrency))
                                        .ToArray();
                            });

                        return mockCurrencyConverter;
                }

                private static Mock<DiscountStrategy> SetupMockDiscountStrategy(Mock<ICurrencyConverter> mockCurrencyConverter, Money returnPrice)
                {
                        var mockStrategy = new Mock<DiscountStrategy>(mockCurrencyConverter.Object, "Test Strategy Name")
                        {
                                CallBase = true // Required so GetFinalPrice executes and routes down to CalculatePrice
                        };

                        // CalculatePrice is a protected abstract method, so we must use Moq.Protected to set it up
                        mockStrategy.Protected()
                            .Setup<Money>(
                                "CalculatePrice",
                                ItExpr.IsAny<Money>(),
                                ItExpr.IsAny<Money>(),
                                ItExpr.IsAny<TreatmentId>(),
                                ItExpr.IsAny<Month?>(),
                                ItExpr.IsAny<List<DateTime>>())
                            .Returns(returnPrice);

                        return mockStrategy;
                }

                private static Campaign CreateTestCampaign(DiscountStrategy strategy, CampaignId? forcedId = null)
                {
                        return new Campaign(
                            forcedId ?? new CampaignId(Guid.NewGuid()),
                            "Test Campaign",
                            "Description for testing purposes",
                            DateTime.UtcNow.AddDays(-10),
                            DateTime.UtcNow.AddDays(10),
                            strategy,
                            TimeSpan.FromDays(30)
                        );
                }
        }
}
