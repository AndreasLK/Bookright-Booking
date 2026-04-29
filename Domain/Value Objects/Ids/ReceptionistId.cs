namespace Domain.Value_Objects.Ids
{
        public record ReceptionistId
        {
                public Guid Value { get; init; }

                public ReceptionistId(Guid value)
                {
                        if (value == Guid.Empty)
                        {
                                throw new ArgumentException(
                                        message: "ID must not be empty",
                                        paramName: nameof(value));
                        }
                        this.Value = value;

                }
        }
}
