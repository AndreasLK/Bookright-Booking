using System.Collections.Concurrent;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System.Threading;
using Domain.Interfaces;

namespace Use_Case.BestDiscount
{
        /// <summary>
        /// Evaluates all active promotional campaigns concurrently to determine the 
        /// lowest possible price for a booking.
        /// </summary>
        public class DiscountService
        {

                private readonly IBookingRepository _bookingRepository;
                private readonly ITreatmentRepository _treatmentRepository;
                private readonly ICurrencyConverter _currencyConverter;

                /// <summary>
                /// Initializes a new instance of the <see cref="DiscountService"/> class.
                /// </summary>
                /// <param name="bookingRepository">Repository for booking records.</param>
                /// <param name="treatmentRepository">Repository for treatment pricing details.</param>
                /// <param name="currencyConverter">Service to normalize diverse currencies for accurate price comparisons.</param>
                /// <exception cref="ArgumentNullException">Thrown if any dependency is null.</exception>
                public DiscountService(IBookingRepository bookingRepository, ITreatmentRepository treatmentRepository, ICurrencyConverter currencyConverter)
                {
                        ArgumentNullException.ThrowIfNull(bookingRepository);
                        ArgumentNullException.ThrowIfNull(treatmentRepository);
                        ArgumentNullException.ThrowIfNull(currencyConverter);

                        this._bookingRepository = bookingRepository;
                        this._treatmentRepository = treatmentRepository;
                        this._currencyConverter = currencyConverter;
                }

                /// <summary>
                /// Calculates the absolute lowest price available by evaluating all active campaigns in parallel.
                /// </summary>
                /// <param name="context">Aggregated booking and customer data required for calculations.</param>
                /// <returns>The most advantageous price, defaulting to the base price if no better discount exists.</returns>
                /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
                public Money GetBestDiscount(DiscountContext context)
                {
                        ArgumentNullException.ThrowIfNull(context);

                        var calculatedPrices = new ConcurrentBag<Money>();

                        Parallel.ForEach(context.ActiveCampaigns, campaign =>
                                {
                                        List<DateTime> campaignUsage = new List<DateTime>();

                                        if (context.TimeUsedEligbleCampaigns.TryGetValue(campaign.Id, out List<DateTime>? usage))
                                        {
                                                campaignUsage = usage;
                                        }

                                        Money finalPrice = campaign.Strategy.GetFinalPrice(
                                                totalPurchase: context.TotalHistoricalSpend,
                                                currentPurchasePrice: context.BasePrice,
                                                treatmentId: context.TreatmentId,
                                                customerBirthMonth: context.CustomerBirthMonth,
                                                timesUsedCampaign: campaignUsage);

                                        calculatedPrices.Add(finalPrice);
                                });



                        Money? bestDiscountPrice = calculatedPrices.OrderBy(
                                money => this._currencyConverter.Convert(
                                        money: money,
                                        toCurrency: context.BasePrice.Currency)
                                .Value)
                                .FirstOrDefault();

                        if (bestDiscountPrice is null)
                        {
                                return context.BasePrice;
                        }

                        decimal normalizedDiscountValue = this._currencyConverter.Convert(
                                money: bestDiscountPrice,
                                toCurrency: context.BasePrice.Currency)
                                .Value;

                        if (normalizedDiscountValue >= context.BasePrice.Value)
                        {
                                return context.BasePrice;
                        }

                        return bestDiscountPrice;
                }
        }
}
