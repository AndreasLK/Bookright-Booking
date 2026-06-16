using Domain.Value_Objects.Ids;

namespace Domain.Value_Objects.Ids
{
        /// <summary>Unique identifier for a Room.</summary>
        public record RoomId(Guid Value) : StronglyTypedId<Guid>(Value);
}
