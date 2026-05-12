using Domain;
using Domain.Entities;
using Domain.Entities.People;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Use_Case.DiscountCalculator;

namespace Use_Case.BestDiscount
{
        public class DiscountContextFactory
        {
                private readonly ICampaignRepository _campaignRepository;
                private readonly IBookingRepository _bookingRepository;
                private readonly ITreatmentRepository _treatmentRepository;
                private readonly ICustomerRepository _customerRepository;
                private readonly ICurrencyConverter _currencyConverter;


                public DiscountContextFactory(
                        ICampaignRepository campaignRepository,
                        IBookingRepository bookingRepository,
                        ITreatmentRepository treatmentRepository,
                        ICustomerRepository customerRepository,
                        ICurrencyConverter currencyConverter)
                {
                        ArgumentNullException.ThrowIfNull(campaignRepository);
                        ArgumentNullException.ThrowIfNull(bookingRepository);
                        ArgumentNullException.ThrowIfNull(treatmentRepository);
                        ArgumentNullException.ThrowIfNull(customerRepository);
                        ArgumentNullException.ThrowIfNull(currencyConverter);

                        this._campaignRepository = campaignRepository;
                        this._bookingRepository = bookingRepository;
                        this._treatmentRepository = treatmentRepository;
                        this._customerRepository = customerRepository;
                        this._currencyConverter = currencyConverter;

                }

                public async Task<DiscountContext> CreateAsync(
                        CustomerId customerId,
                        BookingId bookingId)
                {
                        IEnumerable<Campaign> activeCampaigns = await this._campaignRepository.GetActiveAsync();

                        Booking? currentBooking = await this._bookingRepository.GetByIdAsync(id: bookingId.Value);
                        ArgumentNullException.ThrowIfNull(currentBooking);

                        Treatment? currentTreatment = await this._treatmentRepository.GetByIdAsync(id: currentBooking.TreatmentId.Value);
                        ArgumentNullException.ThrowIfNull(currentTreatment);

                        Money currentBookingPrice = currentTreatment.Price;


                        Customer? customer = await this._customerRepository.GetByIdAsync(id: customerId.Value);
                        ArgumentNullException.ThrowIfNull(customer);

                        Month customerBirthMonth = (Month)customer.DateOfBirth.Month;



                        IReadOnlyList<Booking> customerBookingsSinceEarliestCooldown = await this.GetBookingsSinceEarliestCooldown(customerId, currentBooking.Timeslot.EndDateTime, activeCampaigns);

                        var result = this.GetHistoricalSpendAndUsage(customerBookingsSinceEarliestCooldown);
                        Money totalHistoricalSpend = result.Item1;
                        Dictionary<CampaignId, List<DateTime>> campaignUsage = result.Item2;

                        return new DiscountContext(
                                BasePrice: currentBookingPrice,
                                TreatmentId: currentBooking.TreatmentId,
                                TotalHistoricalSpend: totalHistoricalSpend,
                                ActiveCampaigns: activeCampaigns.ToList(),
                                TimeUsedEligbleCampaigns: campaignUsage,
                                CustomerBirthMonth: customerBirthMonth
                                );
                }

                private async Task<IReadOnlyList<Booking>> GetBookingsSinceEarliestCooldown(CustomerId customerId, DateTime findCampaignUsageBefore, IEnumerable<Campaign> activeCampaigns)
                {

                        DateTime findCampaignUsageAfter = findCampaignUsageBefore;

                        foreach (Campaign campaign in activeCampaigns)
                        {
                                DateTime cooldownDateTime = findCampaignUsageBefore - campaign.Cooldown;
                                if (cooldownDateTime < findCampaignUsageAfter)
                                {
                                        findCampaignUsageAfter = cooldownDateTime;
                                }
                        }

                        return await this._bookingRepository.FindAsync(
                                specification: new PaidBookingsByCustomerSpecification(
                                        customerId: customerId,
                                        bookingsAfter: findCampaignUsageAfter,
                                        bookingsBefore: findCampaignUsageBefore
                                        )
                                );
                }

                private (Money, Dictionary<CampaignId, List<DateTime>>) GetHistoricalSpendAndUsage(IReadOnlyList<Booking> customerBookingsSinceEarliestCooldown)
                {
                        decimal historicalSpend = 0m;
                        Currency historicalSpendCurrency = Currency.DKK;

                        Dictionary<CampaignId, List<DateTime>> campaignUsage = new Dictionary<CampaignId, List<DateTime>>();

                        foreach (Booking booking in customerBookingsSinceEarliestCooldown)
                        {
                                historicalSpend += this._currencyConverter.Convert(
                                        amount: booking.Paid?.Value ?? 0m,
                                        fromCurrency: booking.Paid?.Currency ?? Currency.DKK,
                                        toCurrency: historicalSpendCurrency).Value;

                                if (booking.AppliedCampaign is not null)
                                {
                                        if (!campaignUsage.ContainsKey(booking.AppliedCampaign))
                                        {
                                                campaignUsage[booking.AppliedCampaign] = new List<DateTime>();
                                        }

                                        campaignUsage[booking.AppliedCampaign].Add(booking.Timeslot.EndDateTime);
                                }

                        }

                        Money historicalSpendMoney = new Money(value: historicalSpend, currency: historicalSpendCurrency);

                        return (historicalSpendMoney, campaignUsage);
                }
        }
}
