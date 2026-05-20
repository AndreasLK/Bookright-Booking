using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;

namespace Infrastructure.Persistence
{
        public class InMemoryRoomRepository : IRoomRepository
        {
                private readonly List<Room> _rooms = new();

                public InMemoryRoomRepository()
                {
                        this.SeedData();
                }

                
                public Task<Room?> GetByIdAsync(Guid id) => Task.FromResult(_rooms.FirstOrDefault(r => r.Id.Value == id));
                public Task<IReadOnlyList<Room>> GetAllAsync() => Task.FromResult<IReadOnlyList<Room>>(_rooms.AsReadOnly());

                // ... (Include Add, Update, Delete, FindAsync identically to the others)

                private void SeedData()
                {
                        // Assuming a constructor is added to the Room class
                        // TreatmentCategory requires instantiation based on your specific Domain design
                        _rooms.Add(new Room(
                            id: new RoomId(Guid.Parse("R1111111-1111-1111-1111-111111111111")),
                            name: "Room A - Massage"
                        // primarilyUsedFor: new TreatmentCategory(...)
                        ));

                        _rooms.Add(new Room(
                            id: new RoomId(Guid.Parse("R2222222-2222-2222-2222-222222222222")),
                            name: "Room B - Therapy"
                        ));
                }
        }
}
