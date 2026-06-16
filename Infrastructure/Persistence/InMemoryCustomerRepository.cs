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
        public class InMemoryCustomerRepository : ICustomerRepository
        {
                private readonly List<Customer> _customers = new();

                public InMemoryCustomerRepository() => this.SeedData();

                public Task<Customer?> GetByIdAsync(Guid id) => Task.FromResult(result: this._customers.FirstOrDefault(predicate: c => c.Id.Value == id));
                public Task<IReadOnlyList<Customer>> GetAllAsync() => Task.FromResult<IReadOnlyList<Customer>>(result: this._customers.AsReadOnly());
                public Task<Customer> AddAsync(Customer entity) { this._customers.Add(item: entity); return Task.FromResult(result: entity); }

                public Task UpdateAsync(Customer entity)
                {
                        int index = this._customers.FindIndex(match: c => c.Id.Value == entity.Id.Value);
                        if (index != -1) this._customers[index] = entity;
                        return Task.CompletedTask;
                }

                public Task<bool> DeleteAsync(Guid id) => Task.FromResult(result: this._customers.RemoveAll(match: c => c.Id.Value == id) > 0);

                public Task<IReadOnlyList<Customer>> FindAsync(Specification<Customer> specification)
                {
                        IQueryable<Customer> query = this._customers.AsQueryable().Where(predicate: specification.ToExpression());
                        if (specification.OrderBy is not null) query = query.OrderBy(keySelector: specification.OrderBy);
                        else if (specification.OrderByDescending is not null) query = query.OrderByDescending(keySelector: specification.OrderByDescending);
                        if (specification.Take.HasValue) query = query.Take(count: specification.Take.Value);
                        return Task.FromResult<IReadOnlyList<Customer>>(result: query.ToList().AsReadOnly());
                }

                private void SeedData()
                {
                        this._customers.Add(item: new Customer(
                                id: new CustomerId(Value: Guid.Parse(input: "D1111111-1111-1111-1111-111111111111")),
                                personalNote: null,
                                importantNote: "Allergisk overfor mandelolie.",
                                preferredPratitionerId: null,
                                preferredGender: null,
                                sygsikringDanmarkMember: true,
                                details: new PersonDetails("Jonathan", "Doe", "He/Him", new DateOnly(1990, 5, 14), new PhoneNumber("555-0101"), new EmailAddress("jonny@example.com"), Gender.Man, "Jonny", null)
                        ));

                        this._customers.Add(item: new Customer(
                                id: new CustomerId(Value: Guid.Parse(input: "D2222222-2222-2222-2222-222222222222")),
                                personalNote: "Foretrækker eftermiddag.",
                                importantNote: null,
                                preferredPratitionerId: null,
                                preferredGender: null,
                                sygsikringDanmarkMember: false,
                                details: new PersonDetails("Elizabeth", "Windsor", "She/They", new DateOnly(1950, 4, 21), new PhoneNumber("555-0103"), new EmailAddress("liz@example.com"), Gender.Woman, "Liz", null)
                        ));

                        this._customers.Add(item: new Customer(
                                id: new CustomerId(Value: Guid.Parse(input: "D3333333-3333-3333-3333-333333333333")),
                                personalNote: "Altid i godt humør.",
                                importantNote: "Dårligt knæ (venstre).",
                                preferredPratitionerId: null,
                                preferredGender: null,
                                sygsikringDanmarkMember: true,
                                details: new PersonDetails("Mette", "Frederiksen", "She/Her", new DateOnly(1977, 11, 19), new PhoneNumber("555-0105"), new EmailAddress("mette@example.com"), Gender.Woman, "Mette", null)
                        ));
                }
        }
}
