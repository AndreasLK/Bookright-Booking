using System.Collections.Concurrent;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System.Threading;
using Domain.Interfaces;

namespace Use_Case.BestDiscount
{
        public class DiscountService
        {

                private readonly IBookingRepository _bookingRepository;
                private readonly ITreatmentRepository _treatmentRepository;
                private readonly ICurrencyConverter _currencyConverter;

                public DiscountService(IBookingRepository bookingRepository, ITreatmentRepository treatmentRepository, ICurrencyConverter currencyConverter)
                {
                        ArgumentNullException.ThrowIfNull(bookingRepository);
                        ArgumentNullException.ThrowIfNull(treatmentRepository);
                        ArgumentNullException.ThrowIfNull(currencyConverter);

                        this._bookingRepository = bookingRepository;
                        this._treatmentRepository = treatmentRepository;
                        this._currencyConverter = currencyConverter;
                }

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
