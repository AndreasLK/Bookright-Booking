namespace Domain.Value_Objects.Ids
{
        public record TreatmentCategoryId
        {
                public Guid Value { get; init; }

                public TreatmentCategoryId(Guid value)
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
