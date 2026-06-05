using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence
{
        /// <summary>
        /// A fully functional in-memory implementation of the booking repository.
        /// Includes 15 seeded bookings with varying states (paid/unpaid, different timeslots).
        /// </summary>
        public class InMemoryBookingRepository : IBookingRepository
        {
                private readonly List<Booking> _bookings = new List<Booking>();

                /// <summary>
                /// Initializes a new instance of the <see cref="InMemoryBookingRepository"/> class.
                /// Seeds 15 fake bookings for UI testing and development.
                /// </summary>
                public InMemoryBookingRepository()
                {
                        this.SeedData();
                }

                /// <inheritdoc/>
                public Task<Booking?> GetByIdAsync(Guid id)
                {
                        Booking? booking = this._bookings.FirstOrDefault(predicate: b => b.Id.Value == id);
                        return Task.FromResult(result: booking);
                }

                /// <inheritdoc/>
                public Task<IReadOnlyList<Booking>> GetAllAsync()
                {
                        IReadOnlyList<Booking> readOnlyList = this._bookings.AsReadOnly();
                        return Task.FromResult(result: readOnlyList);
                }

                /// <inheritdoc/>
                public Task<IReadOnlyList<Booking>> FindAsync(Specification<Booking> specification)
                {
                        ArgumentNullException.ThrowIfNull(argument: specification, paramName: nameof(specification));

                        IQueryable<Booking> query = this._bookings.AsQueryable();

                        query = query.Where(predicate: specification.ToExpression());

                        if (specification.OrderBy is not null)
                        {
                                query = query.OrderBy(keySelector: specification.OrderBy);
                        }
                        else if (specification.OrderByDescending is not null)
                        {
                                query = query.OrderByDescending(keySelector: specification.OrderByDescending);
                        }

                        if (specification.Skip.HasValue)
                        {
                                query = query.Skip(count: specification.Skip.Value);
                        }

                        if (specification.Take.HasValue)
                        {
                                query = query.Take(count: specification.Take.Value);
                        }

                        IReadOnlyList<Booking> results = query.ToList().AsReadOnly();
                        return Task.FromResult(result: results);
                }

                /// <inheritdoc/>
                public Task<Booking> AddAsync(Booking entity)
                {
                        ArgumentNullException.ThrowIfNull(argument: entity, paramName: nameof(entity));

                        this._bookings.Add(item: entity);
                        return Task.FromResult(result: entity);
                }

                /// <inheritdoc/>
                public Task UpdateAsync(Booking entity)
                {
                        ArgumentNullException.ThrowIfNull(argument: entity, paramName: nameof(entity));

                        int index = this._bookings.FindIndex(match: b => b.Id.Value == entity.Id.Value);

                        if (index != -1)
                        {
                                this._bookings[index] = entity;
                        }

                        return Task.CompletedTask;
                }

                /// <inheritdoc/>
                public Task<bool> DeleteAsync(Guid id)
                {
                        int removedCount = this._bookings.RemoveAll(match: b => b.Id.Value == id);
                        bool wasRemoved = removedCount > 0;

                        return Task.FromResult(result: wasRemoved);
                }

                /// <summary>
                /// Seeds the repository with 15 test bookings.
                /// Includes purposeful exceptions to simulate missing relations (e.g., deleted practitioners).
                /// </summary>
                private void SeedData()
                {
                        // 1. Known Clinic IDs extracted directly from InMemoryClinicRepository
                        Guid clinic1Id = Guid.Parse(input: "C1111111-1111-1111-1111-111111111111");
                        Guid clinic2Id = Guid.Parse(input: "C2222222-2222-2222-2222-222222222222");

                        // 2. Deterministic Guids for relations.
                        Guid validPractitionerId = Guid.Parse(input: "B1111111-1111-1111-1111-111111111111");
                        Guid missingPractitionerId = Guid.Parse(input: "DEADBEEF-0000-0000-0000-000000000000"); // The deliberate exception

                        Guid validCustomerId = Guid.Parse(input: "A1111111-1111-1111-1111-111111111111"); // Make sure this exists in InMemoryCustomerRepository!
                        Guid validTreatmentId = Guid.Parse(input: "F1111111-1111-1111-1111-111111111111");
                        Guid validRoomId = Guid.Parse(input: "E1111111-1111-1111-1111-111111111111");

                        DateTime baseDate = DateTime.Today;

                        for (int i = 1; i <= 15; i++)
                        {
                                Guid currentPractitioner = (i == 4 || i == 9) ? missingPractitionerId : validPractitionerId;
                                Guid currentClinic = (i % 2 == 0) ? clinic1Id : clinic2Id;

                                // 3. Create the timeslot first to satisfy the constructor requirement
                                DateTime start = baseDate.AddDays(value: i).AddHours(value: 9 + (i % 5));
                                TimeSlot timeslot = new TimeSlot(startDateTime: start, endDateTime: start.AddHours(value: 1));

                                // 4. Instantiate with the required parameters matching the domain entity
                                Booking booking = new Booking(
                                    id: new BookingId(Value: Guid.NewGuid()),
                                    clinic: new ClinicId(Value: currentClinic),
                                    practitioner: new PractitionerId(Value: currentPractitioner),
                                    treatment: new TreatmentId(Value: validTreatmentId),
                                    room: new RoomId(Value: validRoomId),
                                    customer: new CustomerId(Value: validCustomerId),
                                    timeslot: timeslot
                                );

                                // 5. Leave every 4th booking unpaid
                                if (i % 4 != 0)
                                {
                                        Money payment = new Money(value: 500m + (i * 10), currency: Currency.DKK);
                                        booking.RegisterPayment(payment: payment);
                                }

                                this._bookings.Add(item: booking);
                        }
                }
        }
}
