namespace Domain.Value_Objects
{
        /// <summary>
        /// Represents a validated email address within the domain.
        /// </summary>
        public record EmailAddress
        {
                /// <summary>
                /// The validated email string.
                /// </summary>
                public string Value { get; init; }

                /// <summary>
                /// Initializes a new instance of the <see cref="EmailAddress"/> record.
                /// </summary>
                /// <param name="value">The raw email string to validate.</param>
                /// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
                /// <exception cref="ArgumentException">Thrown if the value is empty or lacks the required verification character.</exception>
                public EmailAddress(string value)
                {
                        if (value is null)
                        {
                                throw new ArgumentNullException(paramName: nameof(value));
                        }

                        if (string.IsNullOrWhiteSpace(value: value))
                        {
                                throw new ArgumentException(
                                        message: "Email address cannot be empty.",
                                        paramName: nameof(value));
                        }


                        if (!value.Contains(value: Config.EMAIL_VERIFICATION_CHARACHTER))
                        {
                                throw new ArgumentException(
                                        message: $"Value must contain {Config.EMAIL_VERIFICATION_CHARACHTER}",
                                        paramName: nameof(value));
                        }
                        this.Value = value;
                }
        }
}
