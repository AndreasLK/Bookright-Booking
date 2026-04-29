namespace Domain.Value_Objects.Ids
{
        /// <summary>Unique identifier for a Clinic.</summary>
        public record ClinicId(Guid Value) : StronglyTypedId<Guid>(Value);
}
