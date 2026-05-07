using Domain.Enums;
using Domain.Value_Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
        public interface ICurrencyConverter
        {
                public abstract Money Convert(decimal amount, Currency fromCurrency, Currency toCurrency);


                public abstract Money[] ConvertToSame(Money[] values, Currency targetCurrency);
        }
}
