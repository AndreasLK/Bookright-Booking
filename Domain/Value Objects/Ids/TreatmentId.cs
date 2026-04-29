namespace Domain.Value_Objects.Ids
{
        public record TreatmentId(Guid Value) : StronglyTypedId<Guid>(Value);
}
