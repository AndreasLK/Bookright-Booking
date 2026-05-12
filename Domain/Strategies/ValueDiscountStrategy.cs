using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Strategies
{
        public class ValueDiscountStrategy : DiscountStrategy
        {
                public Money FixedDiscount { get; private set; }

                public Money MinimumPurchasedAmount { get; private set; }

                protected ValueDiscountStrategy(
                        Money fixedDiscount,
                        Money minimumPurchasedAmount,
                        ICurrencyConverter currencyConverter,
                        string displayName) : base(
                                currencyConverter: currencyConverter,
                                displayName: displayName)
                {
                        ArgumentNullException.ThrowIfNull(argument: fixedDiscount, paramName: nameof(fixedDiscount));
                        ArgumentNullException.ThrowIfNull(argument: minimumPurchasedAmount, paramName: nameof(minimumPurchasedAmount));

                        this.FixedDiscount = fixedDiscount;
                        this.MinimumPurchasedAmount = minimumPurchasedAmount;
                }



                protected override Money CalculatePrice(Money totalPurchase, Money currentPurchasePrice, TreatmentId treatmentId, Month? customerBirthMonth, List<DateTime> timesUsedCampaign)

                {
                        Money[] currencyConversionResult = this.CurrencyConverter.ConvertToSame( //NOT DRY, FIX LATER TODO: Refactor to avoid code duplication
                                targetCurrency: this.MinimumPurchasedAmount.Currency,
                                values: [totalPurchase, currentPurchasePrice]);

                        totalPurchase = currencyConversionResult[0];
                        currentPurchasePrice = currencyConversionResult[1];


                        if (this.FixedDiscount.Currency != totalPurchase.Currency)
                        {
                                throw new ArgumentException(message: "Currency mismatch between fixed discount and total purchase", paramName: nameof(this.FixedDiscount));
                        }

                        if (currentPurchasePrice.Value < this.MinimumPurchasedAmount.Value) return currentPurchasePrice;

                        return new Money(
                                value: currentPurchasePrice.Value - this.FixedDiscount.Value,
                                currency: currentPurchasePrice.Currency);
                }
        }
}
