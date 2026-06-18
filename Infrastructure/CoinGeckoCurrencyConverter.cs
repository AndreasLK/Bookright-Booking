using System.Net.Http.Json;
using System.Text.Json;
using Domain;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Value_Objects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
        /// <summary>
        /// Fetches live exchange rates from the CoinGecko public API.
        /// Falls back to configured rates in appsettings.json if the API is unavailable.
        /// Caches each rate in memory to avoid exceeding CoinGecko's rate limits.
        /// </summary>
        public class CoinGeckoCurrencyConverter : ICurrencyConverter
        {
                // Maps our Currency enum values to the coin IDs that CoinGecko uses in its API.
                // CoinGecko identifies coins by name, not ticker symbol.
                // Example: we say Currency.BTC — CoinGecko wants "bitcoin"
                private static readonly Dictionary<Currency, string> COIN_GECKO_IDS = new Dictionary<Currency, string>
        {
            { Currency.BTC,   "bitcoin"        },
            { Currency.Trump, "official-trump" }
        };


                private readonly HttpClient _httpClient;
                private readonly IMemoryCache _cache;
                private readonly ILogger<CoinGeckoCurrencyConverter> _logger;
                private readonly IConfiguration _configuration;

                public CoinGeckoCurrencyConverter(
                    HttpClient httpClient,
                    IMemoryCache cache,
                    ILogger<CoinGeckoCurrencyConverter> logger,
                    IConfiguration configuration)
                {
                        this._httpClient = httpClient;
                        this._cache = cache;
                        this._logger = logger;
                        this._configuration = configuration;

                        // Read timeout from config — defaults to 5 seconds if not set.
                        // This ensures the app does not hang if CoinGecko is slow
                        this._httpClient.Timeout = TimeSpan.FromSeconds(Config.EXCHANGE_RATE_TIMEOUT_SECONDS);
                }

                /// <summary>
                /// Fetches the current exchange rate from CoinGecko.
                /// Returns a fallback rate from appsettings.json if the API is unavailable.
                /// </summary>
                public async Task<decimal> GetLiveRateAsync(
                    Currency fromCurrency,
                    Currency toCurrency,
                    CancellationToken ct = default)
                {
                        // If both currencies are the same, the rate is always 1.
                        if (fromCurrency == toCurrency) return 1m;

                        // We only support converting FROM DKK to a crypto currency.
                        // If anyone asks for something else, use fallback immediately.
                        if (fromCurrency != Currency.DKK || !COIN_GECKO_IDS.ContainsKey(toCurrency))
                        {
                                this._logger.LogWarning(message: "No live rate available for {From} to {To}. Using fallback.", fromCurrency, toCurrency);
                                return this.GetFallbackRate(toCurrency);
                        }

                        // Check if we already have a fresh rate saved in memory.
                        // This prevents us from calling CoinGecko on every single page load.
                        var cacheKey = $"rate_{fromCurrency}_{toCurrency}";
                        if (this._cache.TryGetValue(cacheKey, out decimal cachedRate))
                        {
                                this._logger.LogDebug(message: "Returning cached rate for {To}: {Rate}", toCurrency, cachedRate);
                                return cachedRate;
                        }

                        // No cached rate found — fetch a live rate from CoinGecko.
                        try
                        {
                                var rate = await this.FetchFromCoinGeckoAsync(toCurrency, ct);

                                // Save the rate in memory for the configured number of seconds.
                                this._cache.Set(cacheKey, rate, TimeSpan.FromSeconds(Config.EXCHANGE_RATE_CACHE_SECONDS));

                                this._logger.LogInformation(message: "Live rate fetched for {To}: {Rate}", toCurrency, rate);
                                return rate;
                        }
                        catch (Exception ex)
                        {
                                // Something went wrong — timeout, no internet, API down.
                                // We never crash the app. Instead we use the fallback rate from appsettings.json.
                                this._logger.LogWarning(ex, message: "CoinGecko request failed for {To}. Using fallback rate.", toCurrency);
                                return this.GetFallbackRate(toCurrency);
                        }
                }

                /// <summary>
                /// Makes the actual HTTP request to CoinGecko and parses the JSON response.
                /// Example response from CoinGecko: { "bitcoin": { "dkk": 612345.00 } }
                /// </summary>
                private async Task<decimal> FetchFromCoinGeckoAsync(Currency toCurrency, CancellationToken ct)
                {
                        var coinId = COIN_GECKO_IDS[toCurrency];

                        // Build the URL with the coin ID and DKK as the target currency.
                        var url = $"https://api.coingecko.com/api/v3/simple/price?ids={coinId}&vs_currencies=dkk";

                        var json = await this._httpClient.GetFromJsonAsync<JsonElement>(url, ct);

                        // CoinGecko returns: how many DKK does 1 coin cost.
                        // We invert it to get: how many coins do you get for 1 DKK.
                        var coinPriceInDkk = json
                            .GetProperty(coinId)
                            .GetProperty("dkk")
                            .GetDecimal();

                        return 1m / coinPriceInDkk;
                }

                /// <summary>
                /// Returns a hardcoded fallback rate from appsettings.json.
                /// Used when CoinGecko is unavailable — ensures the app always works,
                /// even without internet access during an exam.
                /// </summary>
                private decimal GetFallbackRate(Currency toCurrency)
                {
                        var coinPriceInDkk = toCurrency switch
                        {
                                Currency.BTC => Config.FALLBACK_RATE_BTC_DKK,
                                Currency.Trump => Config.FALLBACK_RATE_TRUMP_DKK,
                                _ => 1m
                        };

                        return coinPriceInDkk == 0m ? 1m : 1m / coinPriceInDkk;
                }

                // ---------------------------------------------------------------
                // The three synchronous methods required by ICurrencyConverter.
                // These are identical to CurrencyConvertFixed — we keep them here
                // so this class can fully replace it.
                // ---------------------------------------------------------------

                /// <inheritdoc/>
                public Money Convert(decimal amount, Currency fromCurrency, Currency toCurrency)
                {
                        if (fromCurrency == toCurrency)
                                return new Money(value: amount, currency: fromCurrency);

                        if (fromCurrency == Currency.DKK && toCurrency == Currency.EUR)
                                return new Money(value: amount * 0.134m, currency: toCurrency);

                        if (fromCurrency == Currency.EUR && toCurrency == Currency.DKK)
                                return new Money(value: amount * 7.46m, currency: toCurrency);

                        return new Money(value: amount, currency: fromCurrency);
                }

                /// <inheritdoc/>
                public Money Convert(Money money, Currency toCurrency)
                {
                        return this.Convert(money.Value, money.Currency, toCurrency);
                }

                /// <inheritdoc/>
                public Money[] ConvertToSame(Money[] values, Currency targetCurrency)
                {
                        if (values is null || values.Length == 0) return Array.Empty<Money>();

                        return values
                            .Select(money => this.Convert(money.Value, money.Currency, targetCurrency))
                            .ToArray();
                }
        }
}
