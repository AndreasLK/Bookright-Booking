namespace Domain.Value_Objects.Ids
{
        public record CustomerId(Guid Value) : StronglyTypedId<Guid>(Value);
}
