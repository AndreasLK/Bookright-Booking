using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects.Ids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
        public class InMemoryRoomRepository : IRoomRepository
        {
                private readonly List<Room> _rooms = new();

                public InMemoryRoomRepository() => this.SeedData();

                public Task<Room?> GetByIdAsync(Guid id) => Task.FromResult(result: this._rooms.FirstOrDefault(predicate: r => r.Id.Value == id));
                public Task<IReadOnlyList<Room>> GetAllAsync() => Task.FromResult<IReadOnlyList<Room>>(result: this._rooms.AsReadOnly());
                public Task<Room> AddAsync(Room entity) { this._rooms.Add(item: entity); return Task.FromResult(result: entity); }

                public Task UpdateAsync(Room entity)
                {
                        int index = this._rooms.FindIndex(match: r => r.Id.Value == entity.Id.Value);
                        if (index != -1) this._rooms[index] = entity;
                        return Task.CompletedTask;
                }

                public Task<bool> DeleteAsync(Guid id) => Task.FromResult(result: this._rooms.RemoveAll(match: r => r.Id.Value == id) > 0);

                public Task<IReadOnlyList<Room>> FindAsync(Specification<Room> specification)
                {
                        IQueryable<Room> query = this._rooms.AsQueryable().Where(predicate: specification.ToExpression());
                        if (specification.OrderBy is not null) query = query.OrderBy(keySelector: specification.OrderBy);
                        else if (specification.OrderByDescending is not null) query = query.OrderByDescending(keySelector: specification.OrderByDescending);
                        if (specification.Skip.HasValue) query = query.Skip(count: specification.Skip.Value);
                        if (specification.Take.HasValue) query = query.Take(count: specification.Take.Value);
                        return Task.FromResult<IReadOnlyList<Room>>(result: query.ToList().AsReadOnly());
                }

                private void SeedData()
                {
                        TreatmentCategoryId physioCategory = new TreatmentCategoryId(Value: Guid.Parse(input: "A1111111-1111-1111-1111-111111111111"));
                        TreatmentCategoryId massageCategory = new TreatmentCategoryId(Value: Guid.Parse(input: "A2222222-2222-2222-2222-222222222222"));
                        TreatmentCategoryId akuCategory = new TreatmentCategoryId(Value: Guid.Parse(input: "A3333333-3333-3333-3333-333333333333"));
                        TreatmentCategoryId holdCategory = new TreatmentCategoryId(Value: Guid.Parse(input: "A5555555-5555-5555-5555-555555555555"));

                        this._rooms.Add(item: new Room(
                            id: new RoomId(Value: Guid.Parse(input: "AAA11111-1111-1111-1111-111111111111")),
                            name: "Lokale 1 - Fysioterapi",
                            primarilyUsedForId: physioCategory,
                            alsoUsableFor: new List<TreatmentCategoryId> { massageCategory }
                        ));

                        this._rooms.Add(item: new Room(
                            id: new RoomId(Value: Guid.Parse(input: "AAA22222-2222-2222-2222-222222222222")),
                            name: "Lokale 2 - Massage og Velvære",
                            primarilyUsedForId: massageCategory,
                            alsoUsableFor: new List<TreatmentCategoryId>()
                        ));

                        this._rooms.Add(item: new Room(
                            id: new RoomId(Value: Guid.Parse(input: "AAA33333-3333-3333-3333-333333333333")),
                            name: "Lokale 3 - Akupunkturklinikken",
                            primarilyUsedForId: akuCategory,
                            alsoUsableFor: new List<TreatmentCategoryId>()
                        ));

                        this._rooms.Add(item: new Room(
                            id: new RoomId(Value: Guid.Parse(input: "AAA44444-4444-4444-4444-444444444444")),
                            name: "Sal A - Holdtræning",
                            primarilyUsedForId: holdCategory,
                            alsoUsableFor: new List<TreatmentCategoryId> { physioCategory }
                        ));
                }
        }
}
