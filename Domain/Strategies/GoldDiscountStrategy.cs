using Domain.Value_Objects;

namespace Domain.Strategies
{
        public class GoldDiscountStrategy : DiscountStrategy
        {
                public override Money CalculatePrice(Money totalPurchase, Money currentPurchasePrice)
                {
                        decimal currentPurchasePriceWithDiscount = currentPurchasePrice.Amount * Config.GOLD_LOYALTY_DISCOUNT_MULTIPLIER;
                        if (totalPurchase.Amount + currentPurchasePriceWithDiscount < Config.GOLD_LOYALTY_MINIMUM_AMOUNT_PURCHASED)
                        {
                                return currentPurchasePrice;
                        }

                        return new Money(amount: currentPurchasePriceWithDiscount, currency: currentPurchasePrice.Currency);
                }
        }
}
