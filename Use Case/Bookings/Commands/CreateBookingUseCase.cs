using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System;
using System.Collections.Generic;
using System.Text;

namespace Use_Case.Bookings.Commands
{
        /// <summary>
        /// Executes the creation of a new booking, ensuring domain rules and availability are respected.
        /// </summary>
        public class CreateBookingUseCase
        {
                private readonly IBookingRepository _bookingRepository;

                /// <summary>
                /// Initializes a new instance of the CreateBookingUseCase.
                /// </summary>
                /// <param name="bookingRepository">Data access for bookings.</param>
                public CreateBookingUseCase(IBookingRepository bookingRepository)
                {
                        if (bookingRepository is null)
                        {
                                throw new ArgumentNullException(paramName: nameof(bookingRepository));
                        }

                        this._bookingRepository = bookingRepository;
                }

                /// <summary>
                /// Validates the request and persists the new booking to the database.
                /// </summary>
                /// <param name="command">The booking data.</param>
                /// <returns>The unique identifier of the newly created booking.</returns>
                public async Task<Guid> ExecuteAsync(CreateBookingCommand command)
                {
                        if (command is null)
                        {
                                throw new ArgumentNullException(paramName: nameof(command));
                        }

                        if (command.StartDateTime >= command.EndDateTime)
                        {
                                throw new ArgumentException(message: "Start time must be before end time.");
                        }

                        TimeSlot requestedTimeSlot = new TimeSlot(
                            startDateTime: command.StartDateTime,
                            endDateTime: command.EndDateTime
                        );

                        // 1. Concurrency Check: Ensure the room and practitioner are STILL available
                        await this.EnsureNoDoubleBookingAsync(
                            clinicId: command.ClinicId,
                            roomId: command.RoomId,
                            practitionerId: command.PractitionerId,
                            timeSlot: requestedTimeSlot
                        );

                        // 2. Map primitive Guids to Domain Strongly Typed IDs
                        BookingId newBookingId = new BookingId(Value: Guid.NewGuid());
                        CustomerId customerId = new CustomerId(Value: command.CustomerId);
                        ClinicId clinicId = new ClinicId(Value: command.ClinicId);
                        RoomId roomId = new RoomId(Value: command.RoomId);
                        PractitionerId practitionerId = new PractitionerId(Value: command.PractitionerId);
                        TreatmentId treatmentId = new TreatmentId(Value: command.TreatmentId);

                        // 3. Instantiate the Domain Aggregate Root
                        Booking newBooking = new Booking(
                            id: newBookingId,
                            customer: customerId,
                            clinic: clinicId,
                            room: roomId,
                            practitioner: practitionerId,
                            treatment: treatmentId,
                            timeslot: requestedTimeSlot
                        );

                        // 4. Persist to the database
                        await this._bookingRepository.AddAsync(entity: newBooking);

                        return newBookingId.Value;
                }

                /// <summary>
                /// Verifies that no existing booking overlaps with the requested timeslot for the given room or practitioner.
                /// </summary>
                private async Task EnsureNoDoubleBookingAsync(
                    Guid clinicId,
                    Guid roomId,
                    Guid practitionerId,
                    TimeSlot timeSlot)
                {
                        OverlappingBookingsSpecification overlapSpec = new OverlappingBookingsSpecification(
                            clinicId: clinicId,
                            requestedTimeSlot: timeSlot
                        );

                        IReadOnlyList<Booking> overlappingBookings = await this._bookingRepository.FindAsync(specification: overlapSpec);

                        if (overlappingBookings is null || !overlappingBookings.Any())
                        {
                                return;
                        }

                        bool isRoomTaken = overlappingBookings.Any(predicate: b => b.RoomId.Value == roomId);
                        if (isRoomTaken)
                        {
                                throw new InvalidOperationException(message: "The selected room is no longer available for this timeslot.");
                        }

                        bool isPractitionerTaken = overlappingBookings.Any(predicate: b => b.PractitionerId.Value == practitionerId);
                        if (isPractitionerTaken)
                        {
                                throw new InvalidOperationException(message: "The selected practitioner is no longer available for this timeslot.");
                        }
                }
        }
}
