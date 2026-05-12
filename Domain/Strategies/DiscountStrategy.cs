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

                private ManualResetEvent _doneEvent;
                public DiscountStrategy(ICurrencyConverter currencyConverter, string displayName, ManualResetEvent doneEvent)
                {
                        ArgumentNullException.ThrowIfNull(argument: currencyConverter, paramName: nameof(currencyConverter));
                        ArgumentNullException.ThrowIfNull(argument: displayName, paramName: nameof(displayName));
                        ArgumentNullException.ThrowIfNull(argument: doneEvent, nameof(doneEvent));
                        this.CurrencyConverter = currencyConverter;
                        this.DisplayName = displayName;
                        this._doneEvent = doneEvent;
                }
                public Money GetFinalPrice(Money totalPurchase, Money currentPurchasePrice, TreatmentId treatmentId, Month customerBirthMonth, List<DateTime> timesUsedCampaign)
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

                        Money result = this.CalculatePrice(
                                totalPurchase: totalPurchase,
                                currentPurchasePrice: currentPurchasePrice,
                                treatmentId: treatmentId,
                                customerBirthMonth: customerBirthMonth,
                                timesUsedCampaign: timesUsedCampaign);

                        this._doneEvent.Set();
                        return result;

                }
                protected abstract Money CalculatePrice(Money totalPurchase, Money currentPurchasePrice, TreatmentId treatmentId, Month customerBirthMonth, List<DateTime> timesUsedCampaign);



        }
}
