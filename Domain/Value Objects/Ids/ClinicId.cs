namespace Domain.Value_Objects.Ids
{
        public record ClinicId(Guid Value) : StronglyTypedId<Guid>(Value);
}
