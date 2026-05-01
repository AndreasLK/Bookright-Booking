using Domain.Value_Objects.Ids;

namespace Domain.Value_Objects
{
        /// <summary>Unique identifier for a Booking.</summary>
        public record BookingId(Guid Value) : StronglyTypedId<Guid>(Value);
}
