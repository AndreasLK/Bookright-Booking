using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Entities
{
        public class Room
        {
                public RoomId Id { get; init; }
                public string Name { get; private set; }
                public TreatmentCategory PrimarilyUsedFor { get; private set; }

                public List<TreatmentCategoryId> AlsoUsableFor = new List<TreatmentCategoryId>();
        }
}
