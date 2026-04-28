using Domain.Enums;

namespace Domain.Value_Objects
{
        /// <summary>
        /// Represents a monetary value with a specific Currency.
        /// </summary>
        public record Money
        {
                /// <summary>The numerical value of the money. Must be zero or positive.</summary>
                public decimal Amount { get; init; }

                /// <summary>The Currency associated with the Amount.</summary>
                public Currency Currency { get; init; }

                public Money(decimal amount, Currency currency)
                {
                        if (amount < 0)
                        {
                                throw new ArgumentException("Monetary amounts cannot be negative", nameof(amount));
                        }

                        this.Amount = amount;
                        this.Currency = currency;
                }
        }
}
