using Domain.Enums;
using Domain.Value_Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
        /// <summary>
        /// Service contract for converting monetary values between different currencies.
        /// </summary>
        public interface ICurrencyConverter
        {
                /// <summary>
                /// Converts a raw numeric amount from an origin currency to a target currency.
                /// </summary>
                /// <param name="amount">Numeric value to convert.</param>
                /// <param name="fromCurrency">Original currency of the amount.</param>
                /// <param name="toCurrency">Target currency for the conversion.</param>
                /// <returns>Converted amount wrapped in a new Money object.</returns>
                public abstract Money Convert(decimal amount, Currency fromCurrency, Currency toCurrency);

                /// <summary>
                /// Converts an existing Money object to a new target currency.
                /// </summary>
                /// <param name="money">Monetary value to convert.</param>
                /// <param name="toCurrency">Target currency for the conversion.</param>
                /// <returns>New Money object representing the converted value.</returns>
                public abstract Money Convert(Money money, Currency toCurrency);

                /// <summary>
                /// Normalizes an array of Money objects into a single specified target currency.
                /// </summary>
                /// <param name="values">Array of monetary values to normalize.</param>
                /// <param name="targetCurrency">Currency to convert all values into.</param>
                /// <returns>Array of normalized Money objects.</returns>
                public abstract Money[] ConvertToSame(Money[] values, Currency targetCurrency);
        }
}
