namespace Domain.Value_Objects.Ids
{
        public record PractitionerId(Guid Value) : StronglyTypedId<Guid>(Value);
}
