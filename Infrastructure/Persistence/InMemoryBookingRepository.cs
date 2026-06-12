using Domain.Entities;
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

                public Task<Booking?> GetByIdAsync(Guid id) => Task.FromResult(this._bookings.FirstOrDefault(b => b.Id.Value == id));
                public Task<IReadOnlyList<Booking>> GetAllAsync() => Task.FromResult<IReadOnlyList<Booking>>(this._bookings.AsReadOnly());
                public Task<Booking> AddAsync(Booking entity) { this._bookings.Add(entity); return Task.FromResult(entity); }
                public Task UpdateAsync(Booking entity)
                {
                        int index = this._bookings.FindIndex(b => b.Id.Value == entity.Id.Value);
                        if (index != -1) this._bookings[index] = entity;
                        return Task.CompletedTask;
                }
                public Task<bool> DeleteAsync(Guid id) => Task.FromResult(this._bookings.RemoveAll(b => b.Id.Value == id) > 0);

                public Task<IReadOnlyList<Booking>> FindAsync(Specification<Booking> specification)
                {
                        var query = this._bookings.AsQueryable().Where(specification.ToExpression());
                        if (specification.OrderBy is not null) query = query.OrderBy(specification.OrderBy);
                        else if (specification.OrderByDescending is not null) query = query.OrderByDescending(specification.OrderByDescending);
                        if (specification.Skip.HasValue) query = query.Skip(specification.Skip.Value);
                        if (specification.Take.HasValue) query = query.Take(specification.Take.Value);
                        return Task.FromResult<IReadOnlyList<Booking>>(query.ToList().AsReadOnly());
                }

                private void SeedData()
                {
                        Guid clinic1Id = Guid.Parse("C1111111-1111-1111-1111-111111111111");
                        Guid practitioner1Id = Guid.Parse("B1111111-1111-1111-1111-111111111111");
                        Guid treatment1Id = Guid.Parse("F1111111-1111-1111-1111-111111111111");
                        Guid roomId1 = Guid.Parse("AAA11111-1111-1111-1111-111111111111");
                        Guid customer1Id = Guid.Parse("A1111111-1111-1111-1111-111111111111");

                        for (int i = 0; i < 30; i++)
                        {
                                var start = DateTime.Today.AddDays(i % 5).AddHours(8 + (i % 8));
                                this._bookings.Add(new Booking(
                                    id: new BookingId(Guid.NewGuid()),
                                    clinic: new ClinicId(clinic1Id),
                                    practitioner: new PractitionerId(practitioner1Id),
                                    treatment: new TreatmentId(treatment1Id),
                                    room: new RoomId(roomId1),
                                    customer: new CustomerId(customer1Id),
                                    timeslot: new TimeSlot(start, start.AddMinutes(45))
                                ));
                        }
                }
        }
}
