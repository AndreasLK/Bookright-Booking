using Domain.Enums;
using Domain.Value_Objects.Ids;

namespace Domain.Entities
{
        public class TreatmentCategory
        {
                public TreatmentCategoryId Id { get; private set; }
                public string Name { get; private set; }
                public AuthorizationType Authorization { get; private set; }

        }
}
