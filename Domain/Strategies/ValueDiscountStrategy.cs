using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Strategies
{
        /// <summary>
        /// Applies a fixed monetary discount if the current purchase meets a specific minimum threshold.
        /// </summary>
        public class ValueDiscountStrategy : DiscountStrategy
        {
                /// <summary>
                /// Fixed monetary amount to subtract from the treatment price.
                /// </summary>
                public Money FixedDiscount { get; private set; }

                /// <summary>
                /// Required minimum spend for the current treatment to qualify for the discount.
                /// </summary>
                public Money MinimumPurchasedAmount { get; private set; }

                /// <summary>
                /// Initializes a new instance of the <see cref="ValueDiscountStrategy"/> class.
                /// </summary>
                /// <param name="fixedDiscount">Fixed monetary amount to subtract.</param>
                /// <param name="minimumPurchasedAmount">Required minimum spend threshold.</param>
                /// <param name="currencyConverter">Service for normalizing currencies.</param>
                /// <param name="displayName">User-friendly strategy name.</param>
                /// <exception cref="ArgumentNullException">Thrown if <paramref name="fixedDiscount"/> or <paramref name="minimumPurchasedAmount"/> is null.</exception>
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


                /// <summary>
                /// Evaluates the current purchase price against the minimum threshold and applies the fixed discount if eligible.
                /// </summary>
                /// <param name="totalPurchase">Normalized total historical spend.</param>
                /// <param name="currentPurchasePrice">Normalized base price of the treatment.</param>
                /// <param name="treatmentId">Unique identifier of the treatment.</param>
                /// <param name="customerBirthMonth">Optional birth month of the customer.</param>
                /// <param name="timesUsedCampaign">Historical usage dates of this campaign.</param>
                /// <returns>Discounted price if the threshold is met; otherwise, original base price.</returns>
                /// <exception cref="ArgumentException">Thrown if there is a currency mismatch between the fixed discount and the purchase.</exception>
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
