using Domain.Value_Objects;

namespace Domain.Strategies
{
        public class GoldDiscountStrategy : ProcentageDiscountStrategy


        {
                public override decimal DiscountMultiplier => Config.GOLD_LOYALTY_DISCOUNT_MULTIPLIER;

                public override decimal MinimumPurchasedAmount => Config.GOLD_LOYALTY_DISCOUNT_MULTIPLIER;
        }
}
