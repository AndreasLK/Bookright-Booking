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

                public Money[] ConvertToSame(Money[] values, Currency targetCurrency)
                {
                        throw new NotImplementedException();
                }
        }
}
