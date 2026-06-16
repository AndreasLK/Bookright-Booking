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

                public Task<Clinic?> GetByIdAsync(Guid id) => Task.FromResult(result: this._clinics.FirstOrDefault(predicate: c => c.Id.Value == id));
                public Task<IReadOnlyList<Clinic>> GetAllAsync() => Task.FromResult<IReadOnlyList<Clinic>>(result: this._clinics.AsReadOnly());
                public Task<Clinic> AddAsync(Clinic entity) { this._clinics.Add(item: entity); return Task.FromResult(result: entity); }

                public Task UpdateAsync(Clinic entity)
                {
                        int index = this._clinics.FindIndex(match: c => c.Id.Value == entity.Id.Value);
                        if (index != -1) this._clinics[index] = entity;
                        return Task.CompletedTask;
                }

                public Task<bool> DeleteAsync(Guid id) => Task.FromResult(result: this._clinics.RemoveAll(match: c => c.Id.Value == id) > 0);

                public Task<IReadOnlyList<Clinic>> FindAsync(Specification<Clinic> specification)
                {
                        var query = this._clinics.AsQueryable().Where(predicate: specification.ToExpression());
                        return Task.FromResult<IReadOnlyList<Clinic>>(result: query.ToList().AsReadOnly());
                }

                private void SeedData()
                {
                        this._clinics.Add(item: new Clinic(
                            id: new ClinicId(Value: Guid.Parse(input: "C1111111-1111-1111-1111-111111111111")),
                            name: "Vejle Hovedklinik",
                            address: new Address(StreetAddress: "Strandvejen 1", PostalCode: "7100", Country: "Danmark"),
                            phoneNumber: new PhoneNumber(value: "75820000"),
                            email: new EmailAddress(value: "vejle@bookright.dk")
                        ));

                        this._clinics.Add(item: new Clinic(
                            id: new ClinicId(Value: Guid.Parse(input: "C2222222-2222-2222-2222-222222222222")),
                            name: "Kolding Fysioterapi",
                            address: new Address(StreetAddress: "Jernbanegade 15", PostalCode: "6000", Country: "Danmark"),
                            phoneNumber: new PhoneNumber(value: "75500000"),
                            email: new EmailAddress(value: "kolding@bookright.dk")
                        ));

                        this._clinics.Add(item: new Clinic(
                            id: new ClinicId(Value: Guid.Parse(input: "C3333333-3333-3333-3333-333333333333")),
                            name: "Fredericia Wellness",
                            address: new Address(StreetAddress: "Prinsessegade 34", PostalCode: "7000", Country: "Danmark"),
                            phoneNumber: new PhoneNumber(value: "75920000"),
                            email: new EmailAddress(value: "fredericia@bookright.dk")
                        ));
                }
        }
}
