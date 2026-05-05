using Domain.Value_Objects;

namespace Domain.Strategies
{
        public class BronzeDiscountStrategy : ProcentageDiscountStrategy


        {
                public override decimal DiscountMultiplier => Config.BRONZE_LOYALTY_DISCOUNT_MULTIPLIER;

                public override decimal MinimumPurchasedAmount => Config.BRONZE_LOYALTY_DISCOUNT_MULTIPLIER;
        }
}
