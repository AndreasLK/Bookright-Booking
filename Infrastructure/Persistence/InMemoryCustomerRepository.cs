using Domain.Entities.Persons;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Infrastructure.Persistence
{
        /// <summary>
        /// In-memory implementation of ICustomerRepository for development and testing.
        /// Data is stored in a list and lost on application restart.
        /// </summary>
        public class InMemoryCustomerRepository : ICustomerRepository
        {
                private readonly List<Customer> _customers = new();

                public InMemoryCustomerRepository()
                {
                        this.SeedData();
                }

                /// <inheritdoc/>
                public Task<Customer?> GetByIdAsync(Guid id)
                {
                        Customer? found = this._customers.FirstOrDefault(predicate: c => c.Id.Value == id);
                        return Task.FromResult(result: found);
                }

                /// <inheritdoc/>
                public Task<IReadOnlyList<Customer>> GetAllAsync()
                {
                        IReadOnlyList<Customer> all = this._customers.ToList();
                        return Task.FromResult(result: all);
                }

                /// <inheritdoc/>
                public Task<IReadOnlyList<Customer>> FindAsync(Specification<Customer> specification)
                {
                        // Compile expression tree into executable predicate (in-memory only;
                        // EF Core would translate expression directly to SQL)
                        var predicate = specification.ToExpression().Compile();

                        IEnumerable<Customer> query = this._customers.Where(predicate: predicate);

                        if (specification.OrderBy is not null)
                        {
                                query = query.OrderBy(keySelector: specification.OrderBy.Compile());
                        }
                        else if (specification.OrderByDescending is not null)
                        {
                                query = query.OrderByDescending(keySelector: specification.OrderByDescending.Compile());
                        }

                        if (specification.Take.HasValue)
                        {
                                query = query.Take(count: specification.Take.Value);
                        }

                        IReadOnlyList<Customer> result = query.ToList();
                        return Task.FromResult(result: result);
                }

                /// <inheritdoc/>
                public Task<Customer> AddAsync(Customer entity)
                {
                        this._customers.Add(item: entity);
                        return Task.FromResult(result: entity);
                }

                /// <inheritdoc/>
                public Task UpdateAsync(Customer entity)
                {
                        int index = this._customers.FindIndex(match: c => c.Id.Value == entity.Id.Value);
                        if (index >= 0)
                        {
                                this._customers[index] = entity;
                        }
                        return Task.CompletedTask;
                }

                /// <inheritdoc/>
                public Task<bool> DeleteAsync(Guid id)
                {
                        int removedCount = this._customers.RemoveAll(match: c => c.Id.Value == id);
                        bool wasRemoved = removedCount > 0;

                        return Task.FromResult(result: wasRemoved);
                }

                /// <summary>
                /// Populates initial memory state with test customers for development and UI testing.
                /// </summary>
                private void SeedData()
                {
                        PersonDetails jonathanDetails = new PersonDetails(
                            LegalFirstName: "Jonathan",
                            LegalLastName: "Doe",
                            Pronouns: "He/Him",
                            DateOfBirth: new DateOnly(year: 1990, month: 5, day: 14),
                            PhoneNumber: new PhoneNumber(value: "555-0101"),
                            Email: new EmailAddress(value: "jonny@example.com"),
                            Gender: Gender.Man,
                            PreferredFirstName: "Jonny",
                            PreferredLastName: null
                        );

                        Customer jonathan = new Customer(
                            id: new CustomerId(Value: Guid.NewGuid()),
                            personalNote: null,
                            importantNote: "Allergic to certain massage oils.",
                            preferredPratitionerId: null,
                            preferredGender: null,
                            sygsikringDanmarkMember: true,
                            details: jonathanDetails
                        );

                        this._customers.Add(item: jonathan);

                        PersonDetails elizabethDetails = new PersonDetails(
                            LegalFirstName: "Elizabeth",
                            LegalLastName: "Windsor",
                            Pronouns: "She/They",
                            DateOfBirth: new DateOnly(year: 1926, month: 4, day: 21),
                            PhoneNumber: new PhoneNumber(value: "555-0103"),
                            Email: new EmailAddress(value: "liz@example.com"),
                            Gender: Gender.Woman,
                            PreferredFirstName: "Liz",
                            PreferredLastName: "Mountbatten"
                        );

                        Customer elizabeth = new Customer(
                            id: new CustomerId(Value: Guid.NewGuid()),
                            personalNote: "Prefers afternoon appointments.",
                            importantNote: null,
                            preferredPratitionerId: null,
                            preferredGender: null,
                            sygsikringDanmarkMember: false,
                            details: elizabethDetails
                        );

                        this._customers.Add(item: elizabeth);
                }
        }
}
