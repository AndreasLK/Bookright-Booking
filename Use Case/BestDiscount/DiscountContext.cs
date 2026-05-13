using Domain.Entities;
using Domain.Enums;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Use_Case.BestDiscount
{
        /// <summary>
        /// Aggregated contextual data required by the DiscountService 
        /// to evaluate and calculate the best available discount for a customer.
        /// </summary>
        public record DiscountContext
        {
                /// <summary>
                /// Original price of the treatment before any discounts are applied.
                /// </summary>
                public Money BasePrice { get; init; }

                /// <summary>
                /// Unique identifier of the treatment being booked.
                /// </summary>
                public TreatmentId TreatmentId { get; init; }

                /// <summary>
                /// Total amount of money the customer has spent on past completed bookings, 
                /// normalized to a standardized currency.
                /// </summary>
                public Money TotalHistoricalSpend { get; init; }

                /// <summary>
                /// List of currently active promotional campaigns that the system needs to evaluate.
                /// </summary>
                public List<Campaign> ActiveCampaigns { get; init; }

                /// <summary>
                /// Dictionary mapping campaign IDs to a list of dates representing exactly when 
                /// the customer has previously utilized each specific campaign.
                /// </summary>
                public Dictionary<CampaignId, List<DateTime>> TimeUsedEligbleCampaigns { get; init; }

                /// <summary>
                /// Birth month of the customer, used to evaluate birthday-specific promotional strategies. 
                /// Can be null if the customer has not provided a date of birth.
                /// </summary>
                public Month? CustomerBirthMonth { get; init; }

                /// <summary>
                /// Initializes a new instance of the <see cref="DiscountContext"/> record.
                /// </summary>
                /// <param name="basePrice">Original price of the treatment before discounts.</param>
                /// <param name="treatmentId">Unique identifier of the treatment.</param>
                /// <param name="totalHistoricalSpend">Normalized total historical spend of the customer.</param>
                /// <param name="activeCampaigns">List of currently active campaigns. Must not be null.</param>
                /// <param name="timeUsedEligbleCampaigns">Historical usage dates of campaigns for this customer. Must not be null.</param>
                /// <param name="customerBirthMonth">Optional birth month of the customer.</param>
                /// <exception cref="ArgumentNullException">Thrown when <paramref name="activeCampaigns"/> or <paramref name="timeUsedEligbleCampaigns"/> is null.</exception>
                public DiscountContext(
                Money basePrice,
                TreatmentId treatmentId,
                Money totalHistoricalSpend,
                List<Campaign> activeCampaigns,
                Dictionary<CampaignId, List<DateTime>> timeUsedEligbleCampaigns,
                Month? customerBirthMonth
                )
                {
                        ArgumentNullException.ThrowIfNull(argument: activeCampaigns, paramName: nameof(activeCampaigns));
                        ArgumentNullException.ThrowIfNull(argument: timeUsedEligbleCampaigns, paramName: nameof(timeUsedEligbleCampaigns));

                        this.BasePrice = basePrice;
                        this.TreatmentId = treatmentId;
                        this.TotalHistoricalSpend = totalHistoricalSpend;
                        this.ActiveCampaigns = activeCampaigns;
                        this.TimeUsedEligbleCampaigns = timeUsedEligbleCampaigns;
                        this.CustomerBirthMonth = customerBirthMonth;
                }
        }
}
