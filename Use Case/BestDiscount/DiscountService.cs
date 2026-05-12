using System.Collections.Concurrent;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System.Threading;

namespace Use_Case.BestDiscount
{
        public class DiscountService
        {

                private readonly IBookingRepository _bookingRepository;
                private readonly ITreatmentRepository _treatmentRepository;

                public DiscountService(IBookingRepository bookingRepository, ITreatmentRepository treatmentRepository)
                {
                        ArgumentNullException.ThrowIfNull(bookingRepository);
                        ArgumentNullException.ThrowIfNull(treatmentRepository);

                        this._bookingRepository = bookingRepository;
                        this._treatmentRepository = treatmentRepository;
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

                        Money? bestDiscountPrice = calculatedPrices.OrderBy(money => money.Value).FirstOrDefault();

                        if (bestDiscountPrice is null || bestDiscountPrice.Value > context.BasePrice.Value) //TODO: Make sure bestDiscount and basePrice are same currency (they should already be but plz future me check this)
                        {
                                return context.BasePrice;
                        }

                        return bestDiscountPrice;
                }
        }
}
