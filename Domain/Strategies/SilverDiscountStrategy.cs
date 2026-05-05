using Domain.Value_Objects;

namespace Domain.Strategies
{
        public class SilverDiscountStrategy : ProcentageDiscountStrategy


        {
                public override decimal DiscountMultiplier => Config.SILVER_LOYALTY_DISCOUNT_MULTIPLIER;

                public override decimal MinimumPurchasedAmount => Config.SILVER_LOYALTY_DISCOUNT_MULTIPLIER;
        }
}
