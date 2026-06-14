using Domain.Enums;
using Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Tests
{
        public class CoinGeckoCurrencyConverterTests
        {
                private readonly CoinGeckoCurrencyConverter _sut;

                public CoinGeckoCurrencyConverterTests()
                {
                        // We use a real HttpClient here — this is an integration test.
                        // It actually calls CoinGecko to verify the API works.
                        var httpClient = new HttpClient();
                        var cache = new MemoryCache(new MemoryCacheOptions());
                        var logger = NullLogger<CoinGeckoCurrencyConverter>.Instance;

                        this._sut = new CoinGeckoCurrencyConverter(httpClient, cache, logger);
                }

                [Fact]
                public async Task GetLiveRateAsync_BTC_ReturnsPositiveRate()
                {
                        // Act
                        var rate = await this._sut.GetLiveRateAsync(Currency.DKK, Currency.BTC);

                        // Assert — the rate should be a very small positive number
                        // Example: 1 DKK = 0.00000163 BTC
                        Assert.True(rate > 0, $"Expected a positive BTC rate but got: {rate}");
                }

                [Fact]
                public async Task GetLiveRateAsync_Trump_ReturnsPositiveRate()
                {
                        // Act
                        var rate = await this._sut.GetLiveRateAsync(Currency.DKK, Currency.Trump);

                        // Assert
                        Assert.True(rate > 0, $"Expected a positive Trump rate but got: {rate}");
                }

                [Fact]
                public async Task GetLiveRateAsync_SameCurrency_ReturnsOne()
                {
                        // Act
                        var rate = await this._sut.GetLiveRateAsync(Currency.DKK, Currency.DKK);

                        // Assert — same currency should always return exactly 1
                        Assert.Equal(1m, rate);
                }

                [Fact]
                public async Task GetLiveRateAsync_UnsupportedCurrency_ReturnsFallback()
                {
                        // EUR is not in our COIN_GECKO_IDS dictionary.
                        // It should fall back to the Config value immediately without calling the API.
                        var rate = await this._sut.GetLiveRateAsync(Currency.DKK, Currency.EUR);

                        // Assert — fallback for EUR returns 1 (as defined in GetFallbackRate)
                        Assert.True(rate > 0, $"Expected a positive fallback rate but got: {rate}");
                }
        }
}
