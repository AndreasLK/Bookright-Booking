using Domain.Value_Objects.Ids;

namespace Domain.Value_Objects
{
        public record BookingId(Guid Value) : StronglyTypedId<Guid>(Value);
}
