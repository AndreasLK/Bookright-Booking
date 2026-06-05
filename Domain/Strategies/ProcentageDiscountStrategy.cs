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
        /// Applies a percentage-based discount if the customer's combined historical 
        /// and current spend meets a specific monetary threshold.
        /// </summary>
        public class ProcentageDiscountStrategy : DiscountStrategy
        {
                /// <summary>
                /// Decimal factor representing the discount (e.g., 0.85 for 15% off).
                /// </summary>
                public decimal DiscountMultiplier { get; private set; }

                /// <summary>
                /// Required combined spend threshold to qualify for the discount.
                /// </summary>
                public Money MinimumPurchasedAmount { get; private set; }

                /// <summary>
                /// Initializes a new instance of the <see cref="ProcentageDiscountStrategy"/> class.
                /// </summary>
                /// <param name="discountMultiplier">Decimal factor representing the discount.</param>
                /// <param name="minimumPurchasedAmount">Required combined spend threshold.</param>
                /// <param name="currencyConverter">Service for normalizing currencies.</param>
                /// <param name="displayName">User-friendly strategy name.</param>
                /// <exception cref="ArgumentNullException">Thrown if <paramref name="minimumPurchasedAmount"/> is null.</exception>
                protected ProcentageDiscountStrategy(decimal discountMultiplier, Money minimumPurchasedAmount, ICurrencyConverter currencyConverter, string displayName) : base(currencyConverter, displayName)
                {
                        ArgumentNullException.ThrowIfNull(argument: minimumPurchasedAmount, paramName: nameof(minimumPurchasedAmount));

                        this.DiscountMultiplier = discountMultiplier;
                        this.MinimumPurchasedAmount = minimumPurchasedAmount;
                }

                /// <summary>
                /// Evaluates spend thresholds and applies the percentage discount if the customer is eligible.
                /// </summary>
                /// <param name="totalPurchase">Normalized total historical spend.</param>
                /// <param name="currentPurchasePrice">Normalized base price of the treatment.</param>
                /// <param name="treatmentId">Unique identifier of the treatment.</param>
                /// <param name="customerBirthMonth">Optional birth month of the customer.</param>
                /// <param name="timesUsedCampaign">Historical usage dates of this campaign.</param>
                /// <returns>Discounted price if the spend threshold is met; otherwise, original base price.</returns>
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
