using Domain.Entities;
using Domain.Enums;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Use_Case.BestDiscount
{
        public record DiscountContext(
                Money BasePrice,
                TreatmentId TreatmentId,
                Money TotalHistoricalSpend,
                List<Campaign> ActiveCampaigns,
                Dictionary<CampaignId, List<DateTime>> TimeUsedEligbleCampaigns,
                Month? CustomerBirthMonth
                );
}
