using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Use_Case.Bookings
{
        public class CreateBookingUseCase
        {
                private readonly IBookingRepository _bookings;

                /// <summary>
                /// Receives the repository via dependency injection.
                /// This allows us to swap in a fake repository during testing.
                /// </summary>

                public CreateBookingUseCase(IBookingRepository bookings)
                {
                        this._bookings = bookings;
                }

                public async Task<CreateBookingResult> ExecuteAsync(CreateBookingCommand cmd)
                {
                        try
                        {
                                // --- Check 1: Double booking ---
                                // Build a specification describing what we are looking for:
                                // an existing booking that overlaps AND uses one of our resources

                                var overlapSpec = new BookingOverlapSpecification(
                                        roomId: cmd.RoomId,
                                        clinicId: cmd.ClinicId,
                                        practitionerId: cmd.PractitionerId,
                                        timeslot: cmd.Timeslot);

                                var conflicts = await this._bookings.FindAsync(overlapSpec);

                                if (conflicts.Any())
                                {
                                        // Identify exactly WHAT is in conflict for a useful error message
                                        var reasons = new List<string>();

                                        if (conflicts.Any(b => b.RoomId.Value == cmd.RoomId))
                                                reasons.Add("The Room");
                                        if (conflicts.Any(b => b.ClinicId.Value == cmd.ClinicId))
                                                reasons.Add("The clinic");
                                        if (conflicts.Any(b => b.PractitionerId.Value == cmd.PractitionerId))
                                                reasons.Add("The Practitioner");

                                        throw new BookingConflictException(
                                                $"The timeslot is already taken for: {string.Join(", ", reasons)}.");
                                }

                                var clinicSwitchSpec = new PractitionerClinicSwitchSpecification(
                                        practitionerId: cmd.PractitionerId,
                                        intendedClinicId: cmd.ClinicId,
                                        date: DateOnly.FromDateTime(cmd.Timeslot.StartDateTime.DateTime));

                                var clinicConflicts = await this._bookings.FindAsync(clinicSwitchSpec);

                                if (clinicConflicts.Any())
                                {
                                        var existingClinicId = clinicConflicts.First().ClinicId.Value;

                                        throw new PractitionerClinicConflictException(
                                                $"The practitioner is already assigned to a different clinic " +
                                                $"(ID: {existingClinicId}) on this day and cannot switch.");
                                }

                                // When all checks pass, Create booking

                                var newBooking = new Booking(
                                        id: new BookingId(Guid.NewGuid()),
                                        clinic: new ClinicId(cmd.ClinicId),
                                        practitioner: new PractitionerId(cmd.PractitionerId),
                                        treatment: new TreatmentId(cmd.TreatmentId),
                                        room: new RoomId(cmd.RoomId),
                                        timeslot: cmd.Timeslot);

                                await this._bookings.AddAsync(newBooking);

                                return new CreateBookingResult(
                                        Succes: true,
                                        BookingId: newBooking.Id.Value,
                                        ErrorMessage: null);

                        }

                        catch (BookingConflictException ex)
                        {
                                return new CreateBookingResult(
                                        Succes: false,
                                        BookingId: null,
                                        ErrorMessage: ex.Message);
                        }
                        catch (PractitionerClinicConflictException ex)
                        {
                                return new CreateBookingResult(
                                        Succes: false,
                                        BookingId: null,
                                        ErrorMessage: ex.Message);
                        }
                        catch (Exception ex)
                        {
                                return new CreateBookingResult(
                                        Succes: false,
                                        BookingId: null,
                                        ErrorMessage: $"An unexpected error occurred: {ex.Message}");
                        }
                }
        }
}
