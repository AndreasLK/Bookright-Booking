namespace Domain.Value_Objects.Ids
{
        public record CustomerId
        {
                public Guid Id { get; init; }

                public CustomerId(Guid id)
                {
                        if (id == Guid.Empty)
                        {
                                throw new ArgumentException(
                                        message: "ID must not be empty",
                                        paramName: nameof(id));
                        }
                        this.Id = id;

                }
        }
}
