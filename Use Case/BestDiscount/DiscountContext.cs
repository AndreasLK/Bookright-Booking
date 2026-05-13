using Domain.Entities;
using Domain.Enums;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Use_Case.BestDiscount
{
        public record DiscountContext
        {
                public Money BasePrice { get; init; }
                public TreatmentId TreatmentId { get; init; }
                public Money TotalHistoricalSpend { get; init; }
                public List<Campaign> ActiveCampaigns { get; init; }
                public Dictionary<CampaignId, List<DateTime>> TimeUsedEligbleCampaigns { get; init; }
                public Month? CustomerBirthMonth { get; init; }

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
