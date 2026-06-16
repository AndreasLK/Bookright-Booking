using Domain.Entities.Persons;
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
        public class InMemoryPractitionerRepository : IPractitionerRepository
        {
                private readonly List<Practitioner> _practitioners = new();

                public InMemoryPractitionerRepository() => this.SeedData();

                public Task<Practitioner?> GetByIdAsync(Guid id) => Task.FromResult(result: this._practitioners.FirstOrDefault(predicate: p => p.Id.Value == id));
                public Task<IReadOnlyList<Practitioner>> GetAllAsync() => Task.FromResult<IReadOnlyList<Practitioner>>(result: this._practitioners.AsReadOnly());
                public Task<Practitioner> AddAsync(Practitioner entity) { this._practitioners.Add(item: entity); return Task.FromResult(result: entity); }

                public Task UpdateAsync(Practitioner entity)
                {
                        int index = this._practitioners.FindIndex(match: p => p.Id.Value == entity.Id.Value);
                        if (index != -1) this._practitioners[index] = entity;
                        return Task.CompletedTask;
                }

                public Task<bool> DeleteAsync(Guid id) => Task.FromResult(result: this._practitioners.RemoveAll(match: p => p.Id.Value == id) > 0);

                public Task<IReadOnlyList<Practitioner>> FindAsync(Specification<Practitioner> specification)
                {
                        IQueryable<Practitioner> query = this._practitioners.AsQueryable().Where(predicate: specification.ToExpression());
                        if (specification.OrderBy is not null) query = query.OrderBy(keySelector: specification.OrderBy);
                        else if (specification.OrderByDescending is not null) query = query.OrderByDescending(keySelector: specification.OrderByDescending);
                        if (specification.Skip.HasValue) query = query.Skip(count: specification.Skip.Value);
                        if (specification.Take.HasValue) query = query.Take(count: specification.Take.Value);
                        return Task.FromResult<IReadOnlyList<Practitioner>>(result: query.ToList().AsReadOnly());
                }

                private void SeedData()
                {
                        this._practitioners.Add(item: new Practitioner(
                                id: new PractitionerId(Value: Guid.Parse(input: "B1111111-1111-1111-1111-111111111111")),
                                alias: "Sarah (Fysioterapeut)",
                                details: new PersonDetails("Sarah", "Smith", "She/Her", new DateOnly(1985, 8, 22), new PhoneNumber("555-0199"), new EmailAddress("sarah@bookright.dk"), Gender.Woman, "Sarah", null)
                        ));

                        this._practitioners.Add(item: new Practitioner(
                                id: new PractitionerId(Value: Guid.Parse(input: "B2222222-2222-2222-2222-222222222222")),
                                alias: "Mads (Massør)",
                                details: new PersonDetails("Mads", "Mikkelsen", "He/Him", new DateOnly(1990, 3, 15), new PhoneNumber("555-0200"), new EmailAddress("mads@bookright.dk"), Gender.Man, "Mads", null)
                        ));

                        this._practitioners.Add(item: new Practitioner(
                                id: new PractitionerId(Value: Guid.Parse(input: "B3333333-3333-3333-3333-333333333333")),
                                alias: "Sofie (Kost/Aku)",
                                details: new PersonDetails("Sofie", "Lassen", "She/Her", new DateOnly(1988, 11, 5), new PhoneNumber("555-0300"), new EmailAddress("sofie@bookright.dk"), Gender.Woman, "Sofie", null)
                        ));
                }
        }
}
