namespace Domain.Value_Objects.Ids
{
        /// <summary>Unique identifier for a Receptionist.</summary>
        public record ReceptionistId(Guid Value) : StronglyTypedId<Guid>(Value);
}
