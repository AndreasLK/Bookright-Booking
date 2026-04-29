namespace Domain.Value_Objects.Ids
{
        public record ReceptionistId(Guid Value) : StronglyTypedId<Guid>(Value);
}
