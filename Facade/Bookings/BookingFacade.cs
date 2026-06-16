using Domain.Value_Objects;
using System;
using System.Threading.Tasks;
using Use_Case.Bookings.Commands;
using Use_Case.Bookings.Queries;

namespace Facade.Bookings
{
        /// <summary>
        /// Concrete implementation of the booking facade, orchestrating Use Case execution without exposing them to the UI.
        /// </summary>
        public class BookingFacade : IBookingFacade
        {
                private readonly GetAutoAssignmentProposalUseCase _getAutoAssignmentProposalUseCase;
                private readonly CreateBookingUseCase _createBookingUseCase;
                private readonly PayBookingUseCase _payBookingUseCase;

                /// <summary>
                /// Initializes a new instance of the BookingFacade.
                /// </summary>
                public BookingFacade(
                    GetAutoAssignmentProposalUseCase getAutoAssignmentProposalUseCase,
                    CreateBookingUseCase createBookingUseCase,
                    PayBookingUseCase payBookingUseCase)
                {
                        ArgumentNullException.ThrowIfNull(argument: getAutoAssignmentProposalUseCase, paramName: nameof(getAutoAssignmentProposalUseCase));
                        ArgumentNullException.ThrowIfNull(argument: createBookingUseCase, paramName: nameof(createBookingUseCase));
                        ArgumentNullException.ThrowIfNull(argument: payBookingUseCase, paramName: nameof(payBookingUseCase));

                        this._getAutoAssignmentProposalUseCase = getAutoAssignmentProposalUseCase;
                        this._createBookingUseCase = createBookingUseCase;
                        this._payBookingUseCase = payBookingUseCase;
                }

                /// <inheritdoc />
                public async Task<BookingAssignmentFacadeDto> GetAutoAssignmentProposalAsync(
                    Guid customerId,
                    Guid treatmentId,
                    Guid clinicId,
                    DateTime startDateTime,
                    DateTime endDateTime)
                {
                        if (startDateTime >= endDateTime)
                        {
                                throw new ArgumentException(message: "Start time must precede end time.");
                        }

                        TimeSlot requestedTimeSlot = new TimeSlot(
                            startDateTime: startDateTime,
                            endDateTime: endDateTime
                        );

                        BookingAssignmentProposalDto proposal = await this._getAutoAssignmentProposalUseCase.ExecuteAsync(
                            customerId: customerId,
                            treatmentId: treatmentId,
                            clinicId: clinicId,
                            requestedTimeSlot: requestedTimeSlot
                        );

                        return new BookingAssignmentFacadeDto(
                            ProposedRoomId: proposal.ProposedRoomId,
                            ProposedRoomName: proposal.ProposedRoomName,
                            ProposedPractitionerId: proposal.ProposedPractitionerId,
                            ProposedPractitionerName: proposal.ProposedPractitionerName,
                            IsSuccessful: proposal.IsSuccessful
                        );
                }

                /// <inheritdoc />
                public async Task<Guid> CreateBookingAsync(
                    Guid customerId,
                    Guid clinicId,
                    Guid roomId,
                    Guid practitionerId,
                    Guid treatmentId,
                    DateTime startDateTime,
                    DateTime endDateTime)
                {
                        CreateBookingCommand command = new CreateBookingCommand(
                            CustomerId: customerId,
                            ClinicId: clinicId,
                            RoomId: roomId,
                            PractitionerId: practitionerId,
                            TreatmentId: treatmentId,
                            StartDateTime: startDateTime,
                            EndDateTime: endDateTime
                        );

                        Guid newBookingId = await this._createBookingUseCase.ExecuteAsync(command: command);

                        return newBookingId;
                }

                /// <inheritdoc />
                public async Task MarkBookingAsPaidAsync(Guid bookingId)
                {
                        await this._payBookingUseCase.ExecuteAsync(bookingIdRaw: bookingId);
                }
        }
}
