using Domain;
using Domain.Entities;
using Domain.Entities.Persons;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Use_Case.BestDiscount
{
        /// <summary>
        /// Factory responsible for assembling the DiscountContext by aggregating customer history, 
        /// active campaigns, and booking details.
        /// </summary>
        public class DiscountContextFactory
        {
                private readonly ICampaignRepository _campaignRepository;
                private readonly IBookingRepository _bookingRepository;
                private readonly ITreatmentRepository _treatmentRepository;
                private readonly ICustomerRepository _customerRepository;
                private readonly ICurrencyConverter _currencyConverter;

                /// <summary>
                /// Initializes a new instance of the <see cref="DiscountContextFactory"/> class.
                /// </summary>
                /// <param name="campaignRepository">Repository to fetch active promotional campaigns.</param>
                /// <param name="bookingRepository">Repository to fetch booking records.</param>
                /// <param name="treatmentRepository">Repository to fetch treatment pricing details.</param>
                /// <param name="customerRepository">Repository to fetch customer demographic data.</param>
                /// <param name="currencyConverter">Service to normalize diverse currencies into a standardized base currency.</param>
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

                /// <summary>
                /// Assembles and returns a complete DiscountContext for a specific customer and booking.
                /// </summary>
                /// <param name="customerId">Unique identifier of the customer.</param>
                /// <param name="bookingId">Unique identifier of the current booking being evaluated.</param>
                /// <returns>Fully populated DiscountContext ready for discount strategy evaluation.</returns>
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
                                basePrice: currentBookingPrice,
                                treatmentId: currentBooking.TreatmentId,
                                totalHistoricalSpend: totalHistoricalSpend,
                                activeCampaigns: activeCampaigns.ToList(),
                                timeUsedEligbleCampaigns: campaignUsage,
                                customerBirthMonth: customerBirthMonth
                                );
                }

                /// <summary>
                /// Calculates the earliest required cooldown date across all active campaigns to optimize 
                /// database retrieval of past bookings.
                /// </summary>
                /// <param name="customerId">Target customer identifier.</param>
                /// <param name="findCampaignUsageBefore">Upper bound timestamp for the database search.</param>
                /// <param name="activeCampaigns">List of currently active campaigns to evaluate for cooldowns.</param>
                /// <returns>Filtered list of historical bookings falling within the necessary timeframe.</returns>
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

                /// <summary>
                /// Aggregates total historical spend into a single normalized currency (DKK) and extracts 
                /// campaign usage timestamps from past bookings.
                /// </summary>
                /// <param name="customerBookingsSinceEarliestCooldown">List of past bookings to aggregate.</param>
                /// <returns>A tuple containing the normalized total historical spend and a dictionary of campaign usage dates.</returns>
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
