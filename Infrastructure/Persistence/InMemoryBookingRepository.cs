using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
        public class InMemoryBookingRepository : IBookingRepository
        {
                private readonly List<Booking> _bookings = new();

                public InMemoryBookingRepository() => this.SeedData();

                public Task<Booking?> GetByIdAsync(Guid id) => Task.FromResult(result: this._bookings.FirstOrDefault(predicate: b => b.Id.Value == id));
                public Task<IReadOnlyList<Booking>> GetAllAsync() => Task.FromResult<IReadOnlyList<Booking>>(result: this._bookings.AsReadOnly());
                public Task<Booking> AddAsync(Booking entity) { this._bookings.Add(item: entity); return Task.FromResult(result: entity); }

                public Task UpdateAsync(Booking entity)
                {
                        int index = this._bookings.FindIndex(match: b => b.Id.Value == entity.Id.Value);
                        if (index != -1) this._bookings[index] = entity;
                        return Task.CompletedTask;
                }

                public Task<bool> DeleteAsync(Guid id) => Task.FromResult(result: this._bookings.RemoveAll(match: b => b.Id.Value == id) > 0);

                public Task<IReadOnlyList<Booking>> FindAsync(Specification<Booking> specification)
                {
                        IQueryable<Booking> query = this._bookings.AsQueryable().Where(predicate: specification.ToExpression());
                        if (specification.OrderBy is not null) query = query.OrderBy(keySelector: specification.OrderBy);
                        else if (specification.OrderByDescending is not null) query = query.OrderByDescending(keySelector: specification.OrderByDescending);
                        if (specification.Skip.HasValue) query = query.Skip(count: specification.Skip.Value);
                        if (specification.Take.HasValue) query = query.Take(count: specification.Take.Value);
                        return Task.FromResult<IReadOnlyList<Booking>>(result: query.ToList().AsReadOnly());
                }

                private void SeedData()
                {
                        // Repositories Data References
                        ClinicId clinicVejle = new ClinicId(Value: Guid.Parse(input: "C1111111-1111-1111-1111-111111111111"));
                        ClinicId clinicKolding = new ClinicId(Value: Guid.Parse(input: "C2222222-2222-2222-2222-222222222222"));

                        PractitionerId pracSarah = new PractitionerId(Value: Guid.Parse(input: "B1111111-1111-1111-1111-111111111111"));
                        PractitionerId pracMads = new PractitionerId(Value: Guid.Parse(input: "B2222222-2222-2222-2222-222222222222"));
                        PractitionerId pracSofie = new PractitionerId(Value: Guid.Parse(input: "B3333333-3333-3333-3333-333333333333"));

                        RoomId roomPhysio = new RoomId(Value: Guid.Parse(input: "AAA11111-1111-1111-1111-111111111111"));
                        RoomId roomMassage = new RoomId(Value: Guid.Parse(input: "AAA22222-2222-2222-2222-222222222222"));
                        RoomId roomAku = new RoomId(Value: Guid.Parse(input: "AAA33333-3333-3333-3333-333333333333"));

                        CustomerId custJonny = new CustomerId(Value: Guid.Parse(input: "D1111111-1111-1111-1111-111111111111"));
                        CustomerId custLiz = new CustomerId(Value: Guid.Parse(input: "D2222222-2222-2222-2222-222222222222"));
                        CustomerId custMette = new CustomerId(Value: Guid.Parse(input: "D3333333-3333-3333-3333-333333333333"));

                        TreatmentId fysio30 = new TreatmentId(Value: Guid.Parse(input: "F1111111-1111-1111-1111-111111111111"));
                        TreatmentId fysio60 = new TreatmentId(Value: Guid.Parse(input: "F1111111-1111-1111-1111-111111111113"));
                        TreatmentId massage60 = new TreatmentId(Value: Guid.Parse(input: "F2222222-2222-2222-2222-222222222223"));
                        TreatmentId aku45 = new TreatmentId(Value: Guid.Parse(input: "F3333333-3333-3333-3333-333333333333"));

                        DateTime baseDate = DateTime.Today;

                        // 1. Past Booking (Already Paid to show Green UI status)
                        DateTime pastStart = baseDate.AddDays(value: -1).AddHours(value: 9);
                        Booking b1 = new Booking(
                            id: new BookingId(Value: Guid.NewGuid()),
                            clinic: clinicVejle, practitioner: pracSarah, treatment: fysio60, room: roomPhysio, customer: custJonny,
                            timeslot: new TimeSlot(startDateTime: pastStart, endDateTime: pastStart.AddMinutes(value: 60))
                        );
                        b1.RegisterPayment(payment: new Money(value: 745, currency: Currency.DKK));
                        this._bookings.Add(item: b1);

                        // 2. Booking for Today - Unpaid
                        DateTime todayStart1 = baseDate.AddHours(value: 10);
                        this._bookings.Add(item: new Booking(
                            id: new BookingId(Value: Guid.NewGuid()),
                            clinic: clinicVejle, practitioner: pracMads, treatment: massage60, room: roomMassage, customer: custLiz,
                            timeslot: new TimeSlot(startDateTime: todayStart1, endDateTime: todayStart1.AddMinutes(value: 60))
                        ));

                        // 3. Booking for Today - Different Room/Practitioner (Unpaid)
                        DateTime todayStart2 = baseDate.AddHours(value: 13);
                        this._bookings.Add(item: new Booking(
                            id: new BookingId(Value: Guid.NewGuid()),
                            clinic: clinicVejle, practitioner: pracSofie, treatment: aku45, room: roomAku, customer: custMette,
                            timeslot: new TimeSlot(startDateTime: todayStart2, endDateTime: todayStart2.AddMinutes(value: 45))
                        ));

                        // 4. Booking for Tomorrow
                        DateTime tomorrowStart = baseDate.AddDays(value: 1).AddHours(value: 11);
                        this._bookings.Add(item: new Booking(
                            id: new BookingId(Value: Guid.NewGuid()),
                            clinic: clinicKolding, practitioner: pracSarah, treatment: fysio30, room: roomPhysio, customer: custJonny,
                            timeslot: new TimeSlot(startDateTime: tomorrowStart, endDateTime: tomorrowStart.AddMinutes(value: 30))
                        ));

                        // 5. Booking for The Day After Tomorrow
                        DateTime futureStart = baseDate.AddDays(value: 2).AddHours(value: 14);
                        this._bookings.Add(item: new Booking(
                            id: new BookingId(Value: Guid.NewGuid()),
                            clinic: clinicVejle, practitioner: pracMads, treatment: massage60, room: roomMassage, customer: custMette,
                            timeslot: new TimeSlot(startDateTime: futureStart, endDateTime: futureStart.AddMinutes(value: 60))
                        ));
                }
        }
}
