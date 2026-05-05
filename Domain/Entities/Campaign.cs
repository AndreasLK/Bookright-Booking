using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Entities
{
        public abstract class Campaign
        {
                public CampaignId Id { get; private set; }

                public string Name { get; private set; }

                public string Description { get; private set; }

                public DateOnly StartDate { get; private set; }

                public DateOnly EndDate { get; private set; }
                public Money? FixedDiscount { get; private set; }
                public Money? FixedDiscountApplicableAfterSpending { get; private set; }




        }
}
