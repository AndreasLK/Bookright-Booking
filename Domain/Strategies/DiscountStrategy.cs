using Domain.Interfaces;
using Domain.Value_Objects;

namespace Domain.Strategies
{
        public abstract class DiscountStrategy : IDiscountStrategy
        {
                protected ICurrencyConverter CurrencyConverter { get; set; }
                public DiscountStrategy(ICurrencyConverter currencyConverter)
                {
                        ArgumentNullException.ThrowIfNull(argument: currencyConverter, paramName: nameof(currencyConverter));
                        this.CurrencyConverter = currencyConverter;
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
