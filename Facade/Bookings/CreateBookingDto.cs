using System;
using System.Collections.Generic;
using System.Text;

namespace Facade.Bookings
{
        /// <summary>
        /// Contains the necessary data to create a new booking from the UI.
        /// </summary>
        public record CreateBookingDto(
                Guid CustomerId,
                Guid PractitionerId,
                Guid RoomId,
                Guid TreatmentId,
                Guid ClinicId,
                DateTime StartTime,
                DateTime EndTime);
}
