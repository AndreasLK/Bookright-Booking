using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Strategies;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Infrastructure.Persistence
{
        /// <summary>
        /// In-memory repository for managing campaigns, pre-populated with fake data for testing.
        /// </summary>
        public class InMemoryCampaignRepository : InMemoryRepository<Campaign>, ICampaignRepository
        {
                /// <summary>
                /// Initializes a new instance of the <see cref="InMemoryCampaignRepository"/> class and seeds data.
                /// </summary>
                /// <param name="currencyConverter">Injected service for currency normalization needed by the strategies.</param>
                public InMemoryCampaignRepository(ICurrencyConverter currencyConverter)
                {
                        ArgumentNullException.ThrowIfNull(argument: currencyConverter, paramName: nameof(currencyConverter));

                        this.SeedFakeData(currencyConverter: currencyConverter);
                }

                /// <inheritdoc />
                public override Task<Campaign> AddAsync(Campaign entity)
                {
                        ArgumentNullException.ThrowIfNull(argument: entity, paramName: nameof(entity));

                        // Store uses the underlying Guid Value from the strongly-typed CampaignId
                        this.Store.TryAdd(key: entity.Id.Value, value: entity);
                        return Task.FromResult(result: entity);
                }

                /// <inheritdoc />
                public override Task UpdateAsync(Campaign entity)
                {
                        ArgumentNullException.ThrowIfNull(argument: entity, paramName: nameof(entity));

                        // Overwrite the existing entity using its Guid
                        this.Store[entity.Id.Value] = entity;
                        return Task.CompletedTask;
                }

                /// <inheritdoc />
                public async Task<IEnumerable<Campaign>> GetActiveAsync()
                {
                        DateTime now = DateTime.Now;

                        // We fetch all records and filter them in memory
                        IEnumerable<Campaign> allCampaigns = await this.GetAllAsync();

                        IEnumerable<Campaign> activeCampaigns = allCampaigns
                            .Where(predicate: c => c.StartDate <= now && c.EndDate >= now);

                        return activeCampaigns;
                }

                /// <summary>
                /// Generates a variety of fake campaigns for testing purposes.
                /// </summary>
                private void SeedFakeData(ICurrencyConverter currencyConverter)
                {
                        // 1. Summer Promo (15% off, active now)
                        Campaign summerPromo = new Campaign(
                            id: new CampaignId(Value: Guid.NewGuid()),
                            name: "Sommerrabat 2026",
                            description: "Få 15% rabat på alle behandlinger over 500 kr. i hele sommerperioden.",
                            startDate: DateTime.Now.AddMonths(months: -1),
                            endDate: DateTime.Now.AddMonths(months: 2),
                            strategy: new FakePercentageStrategy(
                                discountMultiplier: 0.85m, // 15% off
                                minimumPurchasedAmount: new Money(value: 500m, currency: Currency.DKK),
                                currencyConverter: currencyConverter,
                                displayName: "15% Sommerrabat"
                            ),
                            cooldown: TimeSpan.FromDays(value: 30)
                        );

                        // 2. Welcome Bonus (100 DKK off, active always)
                        Campaign welcomeBonus = new Campaign(
                            id: new CampaignId(Value: Guid.NewGuid()),
                            name: "Velkomstrabat",
                            description: "Få 100 kr. i rabat på din første behandling (minimumskøb på 300 kr).",
                            startDate: DateTime.Now.AddYears(value: -1),
                            endDate: DateTime.Now.AddYears(value: 5),
                            strategy: new FakeValueStrategy(
                                fixedDiscount: new Money(value: 100m, currency: Currency.DKK),
                                minimumPurchasedAmount: new Money(value: 300m, currency: Currency.DKK),
                                currencyConverter: currencyConverter,
                                displayName: "100 kr. Velkomstrabat"
                            ),
                            cooldown: TimeSpan.FromDays(value: 365) // Usable once a year
                        );

                        // 3. Black Friday (25% off, future/expired depending on current date)
                        Campaign blackFriday = new Campaign(
                            id: new CampaignId(Value: Guid.NewGuid()),
                            name: "Black Friday Sale",
                            description: "Kæmpe rabat på 25% for alle behandlinger over 1000 kr.",
                            startDate: new DateTime(year: DateTime.Now.Year, month: 11, day: 20),
                            endDate: new DateTime(year: DateTime.Now.Year, month: 11, day: 30),
                            strategy: new FakePercentageStrategy(
                                discountMultiplier: 0.75m, // 25% off
                                minimumPurchasedAmount: new Money(value: 1000m, currency: Currency.DKK),
                                currencyConverter: currencyConverter,
                                displayName: "25% Black Friday"
                            ),
                            cooldown: TimeSpan.FromDays(value: 1)
                        );

                        // 4. Senior Citizen Discount (Fixed 50 DKK off, active always)
                        Campaign seniorDiscount = new Campaign(
                            id: new CampaignId(Value: Guid.NewGuid()),
                            name: "Pensionistrabat",
                            description: "Fast rabat på 50 kr. til pensionister på alle behandlinger.",
                            startDate: DateTime.Now.AddYears(value: -5),
                            endDate: DateTime.Now.AddYears(value: 5),
                            strategy: new FakeValueStrategy(
                                fixedDiscount: new Money(value: 50m, currency: Currency.DKK),
                                minimumPurchasedAmount: new Money(value: 0m, currency: Currency.DKK),
                                currencyConverter: currencyConverter,
                                displayName: "50 kr. Pensionistrabat"
                            ),
                            cooldown: TimeSpan.FromDays(value: 7)
                        );

                        // Safely adding to the base InMemoryRepository synchronously for seeding purposes
                        this.AddAsync(entity: summerPromo).GetAwaiter().GetResult();
                        this.AddAsync(entity: welcomeBonus).GetAwaiter().GetResult();
                        this.AddAsync(entity: blackFriday).GetAwaiter().GetResult();
                        this.AddAsync(entity: seniorDiscount).GetAwaiter().GetResult();
                }

                /// <summary>
                /// Private wrapper to instantiate the protected ProcentageDiscountStrategy for fake data generation.
                /// </summary>
                private class FakePercentageStrategy : ProcentageDiscountStrategy
                {
                        public FakePercentageStrategy(
                            decimal discountMultiplier,
                            Money minimumPurchasedAmount,
                            ICurrencyConverter currencyConverter,
                            string displayName)
                            : base(
                                discountMultiplier: discountMultiplier,
                                minimumPurchasedAmount: minimumPurchasedAmount,
                                currencyConverter: currencyConverter,
                                displayName: displayName)
                        {
                        }
                }

                /// <summary>
                /// Private wrapper to instantiate the protected ValueDiscountStrategy for fake data generation.
                /// </summary>
                private class FakeValueStrategy : ValueDiscountStrategy
                {
                        public FakeValueStrategy(
                            Money fixedDiscount,
                            Money minimumPurchasedAmount,
                            ICurrencyConverter currencyConverter,
                            string displayName)
                            : base(
                                fixedDiscount: fixedDiscount,
                                minimumPurchasedAmount: minimumPurchasedAmount,
                                currencyConverter: currencyConverter,
                                displayName: displayName)
                        {
                        }
                }
        }
}
