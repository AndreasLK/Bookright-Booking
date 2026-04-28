
using Domain.Value_Objects.Ids;
using Domain.Value_Objects;
namespace Domain.Entities
{
        public abstract class Treatment
        {
                public TreatmentId Id { get; init; }
                public string Name { get; private set; }
                public TreatmentCategoryId CategoryId { get; private set; }
                public Money Price { get; private set; }
        }
}
