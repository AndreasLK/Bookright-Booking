using System;
using System.Collections.Generic;
using System.Text;
using Use_Case.Bookings.Commands;
using Use_Case.Bookings.Queries;

namespace Facade.Bookings
{
        /// <summary>
        /// Defines the contract for booking-related operations, acting as the bridge 
        /// between the UI presentation layer and the underlying Use Cases.
        /// </summary>
        public interface IBookingFacade
        {
                /// <summary>
                /// Orchestrates the automatic selection of a room and practitioner based on domain rules.
                /// </summary>
                /// <param name="customerId">The unique identifier of the customer.</param>
                /// <param name="treatmentId">The unique identifier of the desired treatment.</param>
                /// <param name="clinicId">The unique identifier of the clinic.</param>
                /// <param name="startDateTime">The start time of the requested booking.</param>
                /// <param name="endDateTime">The end time of the requested booking.</param>
                /// <returns>A proposal DTO containing the system's recommendation.</returns>
                public Task<BookingAssignmentProposalDto> GetAutoAssignmentProposalAsync(
                    Guid customerId,
                    Guid treatmentId,
                    Guid clinicId,
                    DateTime startDateTime,
                    DateTime endDateTime);

                /// <summary>
                /// Validates and executes the creation of a new booking.
                /// </summary>
                /// <param name="command">The immutable command containing the raw booking data.</param>
                /// <returns>The unique identifier of the successfully created booking.</returns>
                public Task<Guid> CreateBookingAsync(CreateBookingCommand command);
        }
}
