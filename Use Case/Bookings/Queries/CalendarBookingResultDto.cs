using Domain.Value_Objects.Ids;
using System;

namespace Use_Case.Bookings.Queries
{
        /// <summary>
        /// Pure dto containing the raw data needed to construct a calendar event.
        /// </summary>
        public record CalendarBookingResultDto(
            BookingId BookingId,
            ClinicId ClinicId,
            string TreatmentName,
            string CustomerName,
            DateTime StartTime,
            DateTime EndTime,
            bool IsPaid
        );
}
