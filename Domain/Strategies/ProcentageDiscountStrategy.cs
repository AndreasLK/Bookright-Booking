using System;
using System.Collections.Generic;
using System.Text;
using Domain.Value_Objects;

namespace Domain.Strategies
{
        public abstract class ProcentageDiscountStrategy : DiscountStrategy
        {

                public abstract decimal DiscountMultiplier { get; }
                public abstract decimal MinimumPurchasedAmount { get; }


                protected override Money CalculatePrice(Money totalPurchase, Money currentPurchasePrice)
                {
                        decimal currentPurchasePriceWithDiscount = currentPurchasePrice.Amount * this.DiscountMultiplier;

                        if (totalPurchase.Amount + currentPurchasePriceWithDiscount < this.MinimumPurchasedAmount)
                        {
                                return currentPurchasePrice;
                        }

                        return new Money(amount: currentPurchasePriceWithDiscount, currency: currentPurchasePrice.Currency);
                }
        }

}
