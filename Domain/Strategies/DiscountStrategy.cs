using Domain.Interfaces;
using Domain.Value_Objects;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Domain.Enums;
using Domain.Value_Objects.Ids;

namespace Domain.Strategies
{
        /// <summary>
        /// Base class for implementing discount calculation strategies. Enforces currency 
        /// normalization before executing the specific discount logic.
        /// </summary>
        public abstract class DiscountStrategy : IDiscountStrategy
        {
                /// <summary>
                /// Service used to normalize currencies before calculation.
                /// </summary>
                protected ICurrencyConverter CurrencyConverter { get; private set; }

                /// <summary>
                /// User-friendly name of the discount strategy.
                /// </summary>
                public string DisplayName { get; private set; }

                /// <summary>
                /// Initializes a new instance of the <see cref="DiscountStrategy"/> class.
                /// </summary>
                /// <param name="currencyConverter">Service used for currency normalization.</param>
                /// <param name="displayName">User-friendly name of the strategy.</param>
                /// <exception cref="ArgumentNullException">Thrown if any dependency is null.</exception>
                public DiscountStrategy(ICurrencyConverter currencyConverter, string displayName)
                {
                        ArgumentNullException.ThrowIfNull(argument: currencyConverter, paramName: nameof(currencyConverter));
                        ArgumentNullException.ThrowIfNull(argument: displayName, paramName: nameof(displayName));
                        this.CurrencyConverter = currencyConverter;
                        this.DisplayName = displayName;
                }

                /// <summary>
                /// Normalizes input currencies to a common baseline and calculates the final discounted price.
                /// </summary>
                /// <param name="totalPurchase">Total historical spend of the customer.</param>
                /// <param name="currentPurchasePrice">Base price of the current treatment.</param>
                /// <param name="treatmentId">Unique identifier of the treatment.</param>
                /// <param name="customerBirthMonth">Optional birth month of the customer.</param>
                /// <param name="timesUsedCampaign">Historical usage dates of this campaign for the customer.</param>
                /// <returns>Final calculated price after applying the strategy.</returns>
                /// <exception cref="ArgumentNullException">Thrown if any required parameter is null.</exception>
                public Money GetFinalPrice(Money totalPurchase, Money currentPurchasePrice, TreatmentId treatmentId, Month? customerBirthMonth, List<DateTime> timesUsedCampaign)
                {
                        ArgumentNullException.ThrowIfNull(argument: totalPurchase, paramName: nameof(totalPurchase));
                        ArgumentNullException.ThrowIfNull(argument: currentPurchasePrice, paramName: nameof(currentPurchasePrice));
                        ArgumentNullException.ThrowIfNull(argument: treatmentId, paramName: nameof(treatmentId));
                        ArgumentNullException.ThrowIfNull(argument: timesUsedCampaign, paramName: nameof(timesUsedCampaign));

                        Money[] currencyConversionResult = this.CurrencyConverter.ConvertToSame(
                                targetCurrency: totalPurchase.Currency,
                                values: [totalPurchase, currentPurchasePrice]);

                        totalPurchase = currencyConversionResult[0];
                        currentPurchasePrice = currencyConversionResult[1];

                        return this.CalculatePrice(
                                totalPurchase: totalPurchase,
                                currentPurchasePrice: currentPurchasePrice,
                                treatmentId: treatmentId,
                                customerBirthMonth: customerBirthMonth,
                                timesUsedCampaign: timesUsedCampaign);

                }

                /// <summary>
                /// Executes the specific discount calculation logic implemented by derived classes.
                /// </summary>
                /// <param name="totalPurchase">Normalized total historical spend.</param>
                /// <param name="currentPurchasePrice">Normalized base price of the treatment.</param>
                /// <param name="treatmentId">Unique identifier of the treatment.</param>
                /// <param name="customerBirthMonth">Optional birth month of the customer.</param>
                /// <param name="timesUsedCampaign">Historical usage dates of this campaign.</param>
                /// <returns>Calculated price based on the strategy rules.</returns>
                protected abstract Money CalculatePrice(Money totalPurchase, Money currentPurchasePrice, TreatmentId treatmentId, Month? customerBirthMonth, List<DateTime> timesUsedCampaign);



        }
}
