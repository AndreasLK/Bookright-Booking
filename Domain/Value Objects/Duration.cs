namespace Domain.Value_Objects
{
        public record Duration
        {
                public TimeSpan Value { get; init; }

                public Duration(TimeSpan value)
                {
                        if (value <= TimeSpan.Zero)
                        {
                                throw new ArgumentException(
                                        message: "Duration must be positive",
                                        paramName: nameof(value));
                        }

                        this.Value = value;
                }
        }
}
