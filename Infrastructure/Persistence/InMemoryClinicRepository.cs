using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Infrastructure.Persistence
{
        public class InMemoryClinicRepository : IClinicRepository
        {
                private readonly List<Clinic> _clinics = new();

                public InMemoryClinicRepository()
                {
                        this.SeedData();
                }

                // --- IRepository Methods ---
                public Task<Clinic?> GetByIdAsync(Guid id) => Task.FromResult(this._clinics.FirstOrDefault(c => c.Id.Value == id));
                public Task<IReadOnlyList<Clinic>> GetAllAsync() => Task.FromResult<IReadOnlyList<Clinic>>(this._clinics.AsReadOnly());
                public Task<Clinic> AddAsync(Clinic entity) { this._clinics.Add(entity); return Task.FromResult(entity); }
                public Task UpdateAsync(Clinic entity)
                {
                        int index = this._clinics.FindIndex(c => c.Id.Value == entity.Id.Value);
                        if (index != -1) this._clinics[index] = entity;
                        return Task.CompletedTask;
                }
                public Task<bool> DeleteAsync(Guid id) => Task.FromResult(this._clinics.RemoveAll(c => c.Id.Value == id) > 0);

                public Task<IReadOnlyList<Clinic>> FindAsync(Specification<Clinic> specification)
                {
                        var query = this._clinics.AsQueryable().Where(specification.ToExpression());
                        if (specification.OrderBy != null) query = query.OrderBy(specification.OrderBy);
                        else if (specification.OrderByDescending != null) query = query.OrderByDescending(specification.OrderByDescending);
                        if (specification.Skip.HasValue) query = query.Skip(specification.Skip.Value);
                        if (specification.Take.HasValue) query = query.Take(specification.Take.Value);
                        return Task.FromResult<IReadOnlyList<Clinic>>(query.ToList().AsReadOnly());
                }

                private void SeedData()
                {
                        this._clinics.Add(new Clinic(
                            id: new ClinicId(Guid.Parse("C1111111-1111-1111-1111-111111111111")),
                            name: "Downtown Wellness Center",
                            address: new Address("Main Street 1", "1000", "Country"),
                            phoneNumber: new PhoneNumber("555-0100"),
                            email: new EmailAddress("contact@downtownwellness.com")
                        ));

                        this._clinics.Add(new Clinic(
                            id: new ClinicId(Guid.Parse("C2222222-2222-2222-2222-222222222222")),
                            name: "Uptown Physiotherapy",
                            address: new Address("North Avenue 42", "2000", "Country"),
                            phoneNumber: new PhoneNumber("555-0200"),
                            email: new EmailAddress("hello@uptownphysio.com")
                        ));
                }
        }
}
