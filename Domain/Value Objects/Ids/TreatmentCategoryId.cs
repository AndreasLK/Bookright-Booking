namespace Domain.Value_Objects.Ids
{
        public record TreatmentCategoryId(Guid Value) : StronglyTypedId<Guid>(Value);
}
