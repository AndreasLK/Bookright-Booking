namespace Domain.Value_Objects.Ids
{
        /// <summary>
        /// Provides a base record for strongly-typed identifiers to prevent primitive obsession.
        /// </summary>
        /// <typeparam name="TValue">The underlying primitive type (like string, Guid).</typeparam>
        public abstract record StronglyTypedId<TValue>
                where TValue : notnull
        {
                /// <summary>
                /// Raw underlying value of the identifier.
                /// </summary>
                public TValue Value { get; init; }

                /// <summary>
                /// Initializes a new instance of the <see cref="StronglyTypedId{TValue}"/> record.
                /// </summary>
                /// <param name="value">The primitive value to wrap.</param>
                /// <exception cref="ArgumentNullException">Thrown if value is null.</exception>
                /// <exception cref="ArgumentException">Thrown if value is empty string or empty Guid.</exception>
                protected StronglyTypedId(TValue value)
                {
                        if (value == null) throw new ArgumentNullException(nameof(value));

                        if (value is string s && string.IsNullOrWhiteSpace(s))
                        {
                                throw new ArgumentException(message: "Value cannot be null or whitespace.", paramName: nameof(value));
                        }

                        if (value is Guid g && g == Guid.Empty)
                        {
                                throw new ArgumentException(message: "Value cannot be an empty GUID.", paramName: nameof(value));
                        }

                        this.Value = value;
                }
        }
}
