using System;
using System.Collections.Generic;
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

                        // Act & Assert
                        var exception = Assert.Throws<ArgumentNullException>(() =>
                            new DiscountService(nullBookingRepository!, mockTreatmentRepository.Object));

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

                        // Act & Assert
                        var exception = Assert.Throws<ArgumentNullException>(() =>
                            new DiscountService(mockBookingRepository.Object, nullTreatmentRepository!));

                        Assert.Equal("treatmentRepository", exception.ParamName);
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
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object);

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
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object);

                        var basePrice = new Money(100m, Currency.DKK);

                        var discountContext = new DiscountContext(
                            BasePrice: basePrice,
                            TreatmentId: new TreatmentId(Guid.NewGuid()),
                            TotalHistoricalSpend: new Money(0m, Currency.DKK),
                            ActiveCampaigns: new List<Campaign>(),
                            TimeUsedEligbleCampaigns: new Dictionary<CampaignId, List<DateTime>>(),
                            CustomerBirthMonth: Month.January
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
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object);

                        var mockCurrencyConverter = SetupMockCurrencyConverter();

                        var basePrice = new Money(200m, Currency.DKK);
                        var expectedLowestPrice = new Money(150m, Currency.DKK);

                        var mockStrategyOne = SetupMockDiscountStrategy(mockCurrencyConverter, new Money(180m, Currency.DKK));
                        var mockStrategyTwo = SetupMockDiscountStrategy(mockCurrencyConverter, expectedLowestPrice);

                        var campaignOne = CreateTestCampaign(mockStrategyOne.Object);
                        var campaignTwo = CreateTestCampaign(mockStrategyTwo.Object);

                        var discountContext = new DiscountContext(
                            BasePrice: basePrice,
                            TreatmentId: new TreatmentId(Guid.NewGuid()),
                            TotalHistoricalSpend: new Money(500m, Currency.DKK),
                            ActiveCampaigns: new List<Campaign> { campaignOne, campaignTwo },
                            TimeUsedEligbleCampaigns: new Dictionary<CampaignId, List<DateTime>>(),
                            CustomerBirthMonth: Month.June
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
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object);

                        var mockCurrencyConverter = SetupMockCurrencyConverter();

                        var basePrice = new Money(100m, Currency.DKK);
                        var worseDiscountPrice = new Money(150m, Currency.DKK);

                        var mockStrategy = SetupMockDiscountStrategy(mockCurrencyConverter, worseDiscountPrice);
                        var activeCampaign = CreateTestCampaign(mockStrategy.Object);

                        var discountContext = new DiscountContext(
                            BasePrice: basePrice,
                            TreatmentId: new TreatmentId(Guid.NewGuid()),
                            TotalHistoricalSpend: new Money(200m, Currency.DKK),
                            ActiveCampaigns: new List<Campaign> { activeCampaign },
                            TimeUsedEligbleCampaigns: new Dictionary<CampaignId, List<DateTime>>(),
                            CustomerBirthMonth: Month.August
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
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object);

                        var mockCurrencyConverter = SetupMockCurrencyConverter();
                        var mockStrategy = SetupMockDiscountStrategy(mockCurrencyConverter, new Money(80m, Currency.DKK));

                        var targetCampaignId = new CampaignId(Guid.NewGuid());
                        var expectedUsageHistory = new List<DateTime> { new DateTime(2023, 10, 01) };

                        var activeCampaign = CreateTestCampaign(mockStrategy.Object, targetCampaignId);

                        var usageDictionary = new Dictionary<CampaignId, List<DateTime>>
            {
                { targetCampaignId, expectedUsageHistory }
            };

                        var discountContext = new DiscountContext(
                            BasePrice: new Money(100m, Currency.DKK),
                            TreatmentId: new TreatmentId(Guid.NewGuid()),
                            TotalHistoricalSpend: new Money(1000m, Currency.DKK),
                            ActiveCampaigns: new List<Campaign> { activeCampaign },
                            TimeUsedEligbleCampaigns: usageDictionary,
                            CustomerBirthMonth: Month.December
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
                            ItExpr.Is<List<DateTime>>(list => list == expectedUsageHistory) // Validates the exact usage list was passed
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
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object);

                        var mockCurrencyConverter = SetupMockCurrencyConverter();
                        var mockStrategy = SetupMockDiscountStrategy(mockCurrencyConverter, new Money(50m, Currency.DKK));

                        var activeCampaign = CreateTestCampaign(mockStrategy.Object);

                        var discountContext = new DiscountContext(
                            BasePrice: new Money(100m, Currency.DKK),
                            TreatmentId: new TreatmentId(Guid.NewGuid()),
                            TotalHistoricalSpend: new Money(0m, Currency.DKK),
                            ActiveCampaigns: new List<Campaign> { activeCampaign },
                            TimeUsedEligbleCampaigns: new Dictionary<CampaignId, List<DateTime>>(), // Empty dictionary
                            CustomerBirthMonth: null
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
                            ItExpr.Is<List<DateTime>>(list => list != null && list.Count == 0) // Validates an empty list was passed
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
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object);
                        var mockCurrencyConverter = SetupMockCurrencyConverter();

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
                            BasePrice: basePrice,
                            TreatmentId: new TreatmentId(Guid.NewGuid()),
                            TotalHistoricalSpend: new Money(0m, Currency.DKK),
                            ActiveCampaigns: new List<Campaign> { campaignEur, campaignDkk },
                            TimeUsedEligbleCampaigns: new Dictionary<CampaignId, List<DateTime>>(),
                            CustomerBirthMonth: null
                        );

                        // Act
                        var resultPrice = systemUnderTest.GetBestDiscount(discountContext);

                        // Assert
                        // THIS WILL FAIL. The code does OrderBy(money => money.Value). 
                        // It will see 10 < 50 and return 10 EUR, even though 50 DKK is cheaper!
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
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object);
                        var mockCurrencyConverter = SetupMockCurrencyConverter();

                        // Base price is 50 DKK
                        var basePrice = new Money(50m, Currency.DKK);

                        // Discount is 10 EUR (74.60 DKK). This is WORSE than the base price.
                        var worseEurPrice = new Money(10m, Currency.EUR);
                        var mockStrategyEur = SetupMockDiscountStrategy(mockCurrencyConverter, worseEurPrice);

                        var activeCampaign = CreateTestCampaign(mockStrategyEur.Object);

                        var discountContext = new DiscountContext(
                            BasePrice: basePrice,
                            TreatmentId: new TreatmentId(Guid.NewGuid()),
                            TotalHistoricalSpend: new Money(0m, Currency.DKK),
                            ActiveCampaigns: new List<Campaign> { activeCampaign },
                            TimeUsedEligbleCampaigns: new Dictionary<CampaignId, List<DateTime>>(),
                            CustomerBirthMonth: null
                        );

                        // Act
                        var resultPrice = systemUnderTest.GetBestDiscount(discountContext);

                        // Assert
                        // THIS WILL FAIL. The code compares "10 > 50" (false).
                        // Because 10 is technically smaller than 50, it gives the customer the 10 EUR price, overcharging them!
                        Assert.Equal(basePrice.Value, resultPrice.Value);
                        Assert.Equal(basePrice.Currency, resultPrice.Currency);
                }

                /// <summary>
                /// EXPOSES A BUG: Verifies that if the ActiveCampaigns list in the context is null,
                /// the service handles it gracefully rather than crashing the Parallel.ForEach loop.
                /// </summary>
                [Fact]
                public void GetBestDiscount_WithNullActiveCampaignsList_ThrowsExpectedExceptionOrHandlesGracefully()
                {
                        // Arrange
                        var mockBookingRepository = new Mock<IBookingRepository>();
                        var mockTreatmentRepository = new Mock<ITreatmentRepository>();
                        var systemUnderTest = new DiscountService(mockBookingRepository.Object, mockTreatmentRepository.Object);

                        var discountContext = new DiscountContext(
                            BasePrice: new Money(100m, Currency.DKK),
                            TreatmentId: new TreatmentId(Guid.NewGuid()),
                            TotalHistoricalSpend: new Money(0m, Currency.DKK),
                            ActiveCampaigns: null!, // Intentionally forcing null
                            TimeUsedEligbleCampaigns: new Dictionary<CampaignId, List<DateTime>>(),
                            CustomerBirthMonth: null
                        );

                        // Act & Assert
                        // THIS WILL FAIL. Parallel.ForEach does not check if the source collection is null before executing.
                        // It will throw an unhandled ArgumentNullException originating deep inside System.Threading.Tasks.
                        var exception = Assert.Throws<ArgumentException>(() => systemUnderTest.GetBestDiscount(discountContext));
                        Assert.Contains("ActiveCampaigns cannot be null", exception.Message);
                }



                private static Mock<ICurrencyConverter> SetupMockCurrencyConverter()
                {
                        var mockCurrencyConverter = new Mock<ICurrencyConverter>();

                        // 1. Set up the single Convert method to simulate the exact real business logic
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

                        // 2. Set up the array conversion method
                        // Note: The parameter order here matches the real CurrencyConvertFixed implementation.
                        mockCurrencyConverter
                            .Setup(converter => converter.ConvertToSame(It.IsAny<Money[]>(), It.IsAny<Currency>()))
                            .Returns((Money[] values, Currency targetCurrency) =>
                            {
                                    if (values is null || values.Length == 0)
                                    {
                                            return Array.Empty<Money>();
                                    }

                                    // We call the mocked converter object inside the lambda to route the items 
                                    // through the exact logic we just set up above, just like the real class does.
                                    return values
                    .Select(money => mockCurrencyConverter.Object.Convert(money.Value, money.Currency, targetCurrency))
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
                            id: forcedId ?? new CampaignId(Guid.NewGuid()),
                            name: "Test Campaign",
                            description: "Description for testing purposes",
                            startDate: DateTime.UtcNow.AddDays(-10),
                            endDate: DateTime.UtcNow.AddDays(10),
                            strategy: strategy,
                            cooldown: TimeSpan.FromDays(30)
                        );
                }
        }
}
