using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Strategies
{
        public class ProcentageDiscountStrategy : DiscountStrategy
        {
                public decimal DiscountMultiplier { get; private set; }
                public Money MinimumPurchasedAmount { get; private set; }
                protected ProcentageDiscountStrategy(decimal discountMultiplier, Money minimumPurchasedAmount, ICurrencyConverter currencyConverter, string displayName) : base(currencyConverter, displayName)
                {
                        ArgumentNullException.ThrowIfNull(argument: minimumPurchasedAmount, paramName: nameof(minimumPurchasedAmount));

                        this.DiscountMultiplier = discountMultiplier;
                        this.MinimumPurchasedAmount = minimumPurchasedAmount;
                }

                protected override Money CalculatePrice(Money totalPurchase, Money currentPurchasePrice, TreatmentId treatmentId, Month? customerBirthMonth, List<DateTime> timesUsedCampaign)
                {
                        //Converting both totalPurchase and currentPurchasePrice to the same currency as MinimumPurchasedAmount for comparison

                        Money[] currencyConversionResult = this.CurrencyConverter.ConvertToSame(
                                targetCurrency: this.MinimumPurchasedAmount.Currency,
                                values: [totalPurchase, currentPurchasePrice]);

                        totalPurchase = currencyConversionResult[0];
                        currentPurchasePrice = currencyConversionResult[1];

                        Money currentPurchasePriceWithDiscount = new Money(
                                value: currentPurchasePrice.Value * this.DiscountMultiplier,
                                currency: currentPurchasePrice.Currency);

                        if (totalPurchase.Value + currentPurchasePriceWithDiscount.Value < this.MinimumPurchasedAmount.Value)
                        {
                                return currentPurchasePrice;
                        }

                        return currentPurchasePriceWithDiscount;
                }
        }

}
