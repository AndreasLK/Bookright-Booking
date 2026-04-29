using Domain.Value_Objects.Ids;

namespace Domain.Value_Objects
{
        public record RoomId(Guid Value) : StronglyTypedId<Guid>(Value);
}
