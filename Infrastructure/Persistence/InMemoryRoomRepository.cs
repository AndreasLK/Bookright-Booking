using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Infrastructure.Persistence
{
        public class InMemoryRoomRepository : IRoomRepository
        {
                private readonly List<Room> _rooms = new();

                public InMemoryRoomRepository()
                {
                        this.SeedData();
                }

                public Task<Room?> GetByIdAsync(Guid id)
                {
                        var room = this._rooms.FirstOrDefault(r => r.Id.Value == id);
                        return Task.FromResult(room);
                }

                public Task<IReadOnlyList<Room>> GetAllAsync()
                {
                        IReadOnlyList<Room> readOnlyList = this._rooms.AsReadOnly();
                        return Task.FromResult(result: readOnlyList);
                }

                public Task<Room> AddAsync(Room entity)
                {
                        ArgumentNullException.ThrowIfNull(argument: entity, paramName: nameof(entity));

                        this._rooms.Add(item: entity);
                        return Task.FromResult(result: entity);
                }

                public Task UpdateAsync(Room entity)
                {
                        ArgumentNullException.ThrowIfNull(argument: entity, paramName: nameof(entity));

                        int index = this._rooms.FindIndex(r => r.Id.Value == entity.Id.Value);
                        if (index == -1)
                        {
                                return Task.CompletedTask;
                        }

                        this._rooms[index] = entity;
                        return Task.CompletedTask;
                }

                public Task<bool> DeleteAsync(Guid id)
                {
                        int removedCount = this._rooms.RemoveAll(r => r.Id.Value == id);
                        bool wasRemoved = removedCount > 0;

                        return Task.FromResult(result: wasRemoved);
                }

                public Task<IReadOnlyList<Room>> FindAsync(Specification<Room> specification)
                {
                        ArgumentNullException.ThrowIfNull(argument: specification, paramName: nameof(specification));

                        IQueryable<Room> query = this._rooms.AsQueryable();

                        query = query.Where(specification.ToExpression());

                        if (specification.OrderBy is not null)
                        {
                                query = query.OrderBy(specification.OrderBy);
                        }
                        else if (specification.OrderByDescending is not null)
                        {
                                query = query.OrderByDescending(specification.OrderByDescending);
                        }

                        if (specification.Skip.HasValue)
                        {
                                query = query.Skip(specification.Skip.Value);
                        }

                        if (specification.Take.HasValue)
                        {
                                query = query.Take(specification.Take.Value);
                        }

                        IReadOnlyList<Room> results = query.ToList().AsReadOnly();
                        return Task.FromResult(results);
                }

                private void SeedData()
                {
                        var physioCategoryId = new TreatmentCategoryId(Guid.NewGuid());
                        var massageCategoryId = new TreatmentCategoryId(Guid.NewGuid());

                        this._rooms.Add(new Room(
                            id: new RoomId(Guid.Parse("R1111111-1111-1111-1111-111111111111")),
                            name: "Room 1 - Main Therapy Area",
                            primarilyUsedForId: physioCategoryId,
                            alsoUsableFor: new List<TreatmentCategoryId> { massageCategoryId }
                        ));

                        this._rooms.Add(new Room(
                            id: new RoomId(Guid.Parse("R2222222-2222-2222-2222-222222222222")),
                            name: "Room 2 - Quiet Massage Room",
                            primarilyUsedForId: massageCategoryId,
                            alsoUsableFor: new List<TreatmentCategoryId>()
                        ));
                }
        }
}
