using Domain.Interfaces;
using Domain.Value_Objects;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Domain.Enums;
using Domain.Value_Objects.Ids;

namespace Domain.Strategies
{
        public abstract class DiscountStrategy : IDiscountStrategy
        {
                protected ICurrencyConverter CurrencyConverter { get; private set; }
                public string DisplayName { get; private set; }
                public DiscountStrategy(ICurrencyConverter currencyConverter, string displayName)
                {
                        ArgumentNullException.ThrowIfNull(argument: currencyConverter, paramName: nameof(currencyConverter));
                        ArgumentNullException.ThrowIfNull(argument: displayName, paramName: nameof(displayName));
                        this.CurrencyConverter = currencyConverter;
                        this.DisplayName = displayName;
                }
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
                protected abstract Money CalculatePrice(Money totalPurchase, Money currentPurchasePrice, TreatmentId treatmentId, Month? customerBirthMonth, List<DateTime> timesUsedCampaign);



        }
}
