using System;
using System.Collections.Generic;
using System.Text;
using Domain.Value_Objects;

namespace Domain.Strategies
{
        public abstract class ValueDiscountStrategy : DiscountStrategy
        {
                public abstract decimal FixedDiscount { get; }

                public abstract decimal MinimumPurchasedAmount { get; }

                protected override Money CalculatePrice(Money totalPurchase, Money currentPurchasePrice)

                {
                        decimal currentPurchasePriceWithDiscount = currentPurchasePrice.Amount - this.FixedDiscount;

                        if (currentPurchasePrice.Amount < this.MinimumPurchasedAmount) return currentPurchasePrice;

                        return new Money(amount: currentPurchasePriceWithDiscount, currency: currentPurchasePrice.Currency);
                }
        }
}
