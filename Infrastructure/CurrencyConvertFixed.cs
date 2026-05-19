using Domain.Interfaces;
using Domain.Value_Objects;
using Domain.Enums;

namespace Infrastructure
{
        public class CurrencyConvertFixed : ICurrencyConverter
        {
                public Money Convert(decimal amount, Currency fromCurrency, Currency toCurrency)
                {
                        if (fromCurrency == toCurrency)
                        {
                                return new Money(value: amount, currency: fromCurrency);
                        }
                        else if (fromCurrency == Currency.DKK && toCurrency == Currency.EUR)
                        {
                                return new Money(value: amount * 0.134m, currency: toCurrency);
                        }
                        else if (fromCurrency == Currency.EUR && toCurrency == Currency.DKK)
                        {
                                return new Money(value: amount * 7.46m, currency: toCurrency);
                        }
                        else
                        {
                                return new Money(value: amount, currency: fromCurrency);
                        }
                }

                public Money Convert(Money money, Currency toCurrency)
                {
                        return this.Convert(money.Value, money.Currency, toCurrency);
                }

                public Money[] ConvertToSame(Money[] values, Currency targetCurrency)
                {
                        if (values is null || values.Length == 0) return Array.Empty<Money>();

                        return values
                                .Select(money => this.Convert(money.Value, money.Currency, targetCurrency))
                                .ToArray();
                }
        }
}
