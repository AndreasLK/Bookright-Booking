namespace Domain.Value_Objects.Ids
{
        /// <summary>Unique identifier for a Practitioner.</summary>
        public record PractitionerId(Guid Value) : StronglyTypedId<Guid>(Value);
}
