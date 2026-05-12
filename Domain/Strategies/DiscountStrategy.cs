using Domain.Interfaces;
using Domain.Value_Objects;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

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
                public Money GetFinalPrice(Money totalPurchase, Money currentPurchasePrice)
                {
                        ArgumentNullException.ThrowIfNull(argument: totalPurchase, paramName: nameof(totalPurchase));
                        ArgumentNullException.ThrowIfNull(argument: currentPurchasePrice, paramName: nameof(currentPurchasePrice));

                        Money[] currencyConversionResult = this.CurrencyConverter.ConvertToSame(
                                targetCurrency: totalPurchase.Currency,
                                values: [totalPurchase, currentPurchasePrice]);

                        totalPurchase = currencyConversionResult[0];
                        currentPurchasePrice = currencyConversionResult[1];


                        return this.CalculatePrice(totalPurchase: totalPurchase, currentPurchasePrice: currentPurchasePrice);

                }
                protected abstract Money CalculatePrice(Money totalPurchase, Money currentPurchasePrice);




        }
}
