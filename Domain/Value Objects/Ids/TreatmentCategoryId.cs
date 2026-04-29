namespace Domain.Value_Objects.Ids
{
        /// <summary>Unique identifier for a Treatment Category.</summary>
        public record TreatmentCategoryId(Guid Value) : StronglyTypedId<Guid>(Value);
}
