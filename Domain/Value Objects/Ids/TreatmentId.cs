namespace Domain.Value_Objects.Ids
{
        /// <summary>Unique identifier for a Treatment.</summary>
        public record TreatmentId(Guid Value) : StronglyTypedId<Guid>(Value);
}
