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
        public class InMemoryClinicRepository : IClinicRepository
        {
                private readonly List<Clinic> _clinics = new();

                public InMemoryClinicRepository() => this.SeedData();

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
                        return Task.FromResult<IReadOnlyList<Clinic>>(query.ToList().AsReadOnly());
                }

                private void SeedData()
                {
                        this._clinics.Add(new Clinic(
                            id: new ClinicId(Guid.Parse("C1111111-1111-1111-1111-111111111111")),
                            name: "Vejle Hovedklinik",
                            address: new Address("Strandvejen 1", "7100", "Danmark"),
                            phoneNumber: new PhoneNumber("75820000"),
                            email: new EmailAddress("vejle@bookright.dk")
                        ));
                }
        }
}
