using System;
using System.Collections.Generic;
using System.Text;

namespace Use_Case.Bookings.Queries
{
        /// <summary>
        /// Response DTO containing the system's proposed room and practitioner for a new booking.
        /// If an ID is null, it means the system could not find a match satisfying the rules.
        /// </summary>
        public record BookingAssignmentProposalDto(
            Guid? ProposedRoomId,
            string? ProposedRoomName,
            Guid? ProposedPractitionerId,
            string? ProposedPractitionerName,
            bool IsSuccessful
        );
}
