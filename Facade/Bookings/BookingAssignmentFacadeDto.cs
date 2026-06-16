namespace Facade.Bookings
{
        /// <summary>
        /// Represents a proposal for a booking assignment from the system, adapted for the Facade layer.
        /// </summary>
        /// <param name="ProposedRoomId">The ID of the suggested room, if any.</param>
        /// <param name="ProposedRoomName">The name of the suggested room, if any.</param>
        /// <param name="ProposedPractitionerId">The ID of the suggested practitioner, if any.</param>
        /// <param name="ProposedPractitionerName">The name of the suggested practitioner, if any.</param>
        /// <param name="IsSuccessful">Indicates whether a valid proposal could be generated.</param>
        public record BookingAssignmentFacadeDto(
            Guid? ProposedRoomId,
            string? ProposedRoomName,
            Guid? ProposedPractitionerId,
            string? ProposedPractitionerName,
            bool IsSuccessful
        );
}
