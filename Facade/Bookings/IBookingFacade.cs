using System;
using System.Threading.Tasks;

namespace Facade.Bookings
{
        /// <summary>
        /// Defines the contract for booking-related operations, acting as the strict bridge 
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
                public Task<BookingAssignmentFacadeDto> GetAutoAssignmentProposalAsync(
                    Guid customerId,
                    Guid treatmentId,
                    Guid clinicId,
                    DateTime startDateTime,
                    DateTime endDateTime);

                /// <summary>
                /// Validates and executes the creation of a new booking via the underlying Use Case.
                /// </summary>
                /// <param name="customerId">The ID of the customer.</param>
                /// <param name="clinicId">The ID of the clinic.</param>
                /// <param name="roomId">The ID of the room.</param>
                /// <param name="practitionerId">The ID of the practitioner.</param>
                /// <param name="treatmentId">The ID of the treatment.</param>
                /// <param name="startDateTime">The start time of the booking.</param>
                /// <param name="endDateTime">The end time of the booking.</param>
                /// <returns>The unique identifier of the successfully created booking.</returns>
                public Task<Guid> CreateBookingAsync(
                    Guid customerId,
                    Guid clinicId,
                    Guid roomId,
                    Guid practitionerId,
                    Guid treatmentId,
                    DateTime startDateTime,
                    DateTime endDateTime);

                /// <summary>
                /// Calculates the final price and marks the specified booking as paid.
                /// </summary>
                /// <param name="bookingId">The ID of the booking to pay.</param>
                public Task MarkBookingAsPaidAsync(Guid bookingId);

                /// <summary>
                /// Calculates the estimated final price for a potential booking before it is saved, 
                /// including applicable surcharges and discounts.
                /// </summary>
                /// <param name="customerId">The unique identifier of the customer.</param>
                /// <param name="treatmentId">The unique identifier of the selected treatment.</param>
                /// <param name="startDateTime">The planned start time of the booking.</param>
                /// <param name="endDateTime">The planned end time of the booking.</param>
                /// <returns>The calculated final price as an itemized reciept.</returns>
                public Task<BookingPricingDetailsDto> GetBookingPricePreviewAsync(
                    Guid customerId,
                    Guid treatmentId,
                    DateTime startDateTime,
                    DateTime endDateTime);

                /// <summary>
                /// Calculates the exact final price for an existing booking, 
                /// including applicable surcharges and discounts based on the customer's history.
                /// </summary>
                /// <param name="bookingId">The unique identifier of the saved booking.</param>
                /// <returns>The calculated final price as an itemized reciept.</returns>
                public Task<BookingPricingDetailsDto> GetSavedBookingPriceAsync(Guid bookingId);
        }
}
