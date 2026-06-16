using Domain.Value_Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Use_Case.Bookings.Commands;
using Use_Case.Bookings.Queries;

namespace Facade.Bookings
{
        /// <summary>
        /// Concrete implementation of the booking facade, orchestrating Use Case execution.
        /// </summary>
        public class BookingFacade : IBookingFacade
        {
                private readonly GetAutoAssignmentProposalUseCase _getAutoAssignmentProposalUseCase;
                private readonly CreateBookingUseCase _createBookingUseCase;

                /// <summary>
                /// Initializes a new instance of the BookingFacade.
                /// </summary>
                public BookingFacade(
                    GetAutoAssignmentProposalUseCase getAutoAssignmentProposalUseCase,
                    CreateBookingUseCase createBookingUseCase)
                {
                        ArgumentNullException.ThrowIfNull(argument: getAutoAssignmentProposalUseCase, paramName: nameof(getAutoAssignmentProposalUseCase));
                        ArgumentNullException.ThrowIfNull(argument: createBookingUseCase, paramName: nameof(createBookingUseCase));

                        this._getAutoAssignmentProposalUseCase = getAutoAssignmentProposalUseCase;
                        this._createBookingUseCase = createBookingUseCase;
                }

                /// <inheritdoc />
                public async Task<BookingAssignmentProposalDto> GetAutoAssignmentProposalAsync(
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

                        // Convert UI primitives to Domain Value Object
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

                        return proposal;
                }

                /// <inheritdoc />
                public async Task<Guid> CreateBookingAsync(CreateBookingCommand command)
                {
                        ArgumentNullException.ThrowIfNull(argument: command, paramName: nameof(command));

                        Guid newBookingId = await this._createBookingUseCase.ExecuteAsync(command: command);

                        return newBookingId;
                }
        }
}
