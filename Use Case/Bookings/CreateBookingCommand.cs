using System;
using System.Collections.Generic;
using System.Text;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Use_Case.Bookings
{
        /// <summary>
        /// Contains all data required to create a new booking.
        /// Records are immutable — they cannot be changed after creation.
        /// </summary>
        public record CreateBookingCommand(
                Guid ClinicId,
                Guid PractitionerId,
                Guid TreatmentId,
                Guid RoomId,
                TimeSlot Timeslot);

        /// <summary>
        /// The result returned to the UI layer after attempting to create a booking.
        /// </summary>
        public record CreateBookingResult(
                bool Succes,
                Guid? BookingId,
                string? ErrorMessage);


        
        }
}
