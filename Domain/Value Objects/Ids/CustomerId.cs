namespace Domain.Value_Objects.Ids
{
        /// <summary>Unique identifier for a Customer.</summary>
        public record CustomerId(Guid Value) : StronglyTypedId<Guid>(Value);
}
