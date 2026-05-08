using Domain.Enums;

namespace Domain.Value_Objects
{
        /// <summary>
        /// Represents a monetary value with a specific Currency.
        /// </summary>
        public record Money
        {
                /// <summary>The numerical value of the money. Must be zero or positive.</summary>
                public decimal Value { get; init; }

                /// <summary>The Currency associated with the Value.</summary>
                public Currency Currency { get; init; }

                public Money(decimal value, Currency currency)
                {
                        if (value < 0)
                        {
                                throw new ArgumentException("Monetary amounts cannot be negative", nameof(value));
                        }

                        this.Value = value;
                        this.Currency = currency;
                }
        }
}
