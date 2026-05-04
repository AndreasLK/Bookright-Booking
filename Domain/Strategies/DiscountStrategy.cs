using Domain.Value_Objects;

namespace Domain.Strategies
{
        public abstract class DiscountStrategy
        {
                public Money GetFinalPrice(Money totalPurchase, Money currentPurchasePrice)
                {
                        ArgumentNullException.ThrowIfNull(argument: totalPurchase, paramName: nameof(totalPurchase));
                        ArgumentNullException.ThrowIfNull(argument: currentPurchasePrice, paramName: nameof(currentPurchasePrice));

                        if (totalPurchase.Currency != currentPurchasePrice.Currency) throw new ArgumentException(
                                message: "Currency mismatch between total purchase and current purchase price.",
                                paramName: nameof(currentPurchasePrice));

                        return this.CalculatePrice(totalPurchase: totalPurchase, currentPurchasePrice: currentPurchasePrice);

                }
                public abstract Money CalculatePrice(Money totalPurchase, Money currentPurchasePrice);

        }
}
