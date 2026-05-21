using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Entities
{
        public class Room
        {
                public RoomId Id { get; init; }

                public string Name { get; private set; }

                public TreatmentCategoryId PrimarilyUsedForId { get; private set; }

                public List<TreatmentCategoryId> AlsoUsableFor { get; private set; }

                public Room(
                    RoomId id,
                    string name,
                    TreatmentCategoryId primarilyUsedForId,
                    List<TreatmentCategoryId>? alsoUsableFor = null)
                {
                        ArgumentNullException.ThrowIfNull(id, nameof(id));
                        ArgumentNullException.ThrowIfNull(name, nameof(name));

                        if (string.IsNullOrWhiteSpace(name))
                        {
                                throw new ArgumentException("Room name cannot be empty or whitespace.", nameof(name));
                        }

                        ArgumentNullException.ThrowIfNull(primarilyUsedForId, nameof(primarilyUsedForId));

                        this.Id = id;
                        this.Name = name;
                        this.PrimarilyUsedForId = primarilyUsedForId;
                        this.AlsoUsableFor = alsoUsableFor ?? new List<TreatmentCategoryId>();
                }
        }
}
