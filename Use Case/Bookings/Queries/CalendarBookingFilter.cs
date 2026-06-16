using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System;
using System.Collections.Generic;
using System.Text;

namespace Use_Case.Bookings.Queries
{
        /// <summary>
        /// Represents the multi-select filtering criteria for fetching calendar view data.
        /// </summary>
        public record CalendarBookingFilter(
            DateTime ViewStartDate,
            DateTime ViewEndDate,
            IEnumerable<ClinicId>? ClinicIds = null,
            IEnumerable<RoomId>? RoomIds = null,
            IEnumerable<PractitionerId>? PractitionerIds = null,
            IEnumerable<CustomerId>? CustomerIds = null
        );
}
