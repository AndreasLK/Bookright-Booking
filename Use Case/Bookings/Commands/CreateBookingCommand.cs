using System;
using System.Collections.Generic;
using System.Text;

namespace Use_Case.Bookings.Commands
{
        /// <summary>
        /// Immutable command containing all necessary data to create a new booking.
        /// </summary>
        public record CreateBookingCommand(
            Guid CustomerId,
            Guid ClinicId,
            Guid RoomId,
            Guid PractitionerId,
            Guid TreatmentId,
            DateTime StartDateTime,
            DateTime EndDateTime
        );
}
