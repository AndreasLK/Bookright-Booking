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
                private static readonly List<Customer> CUSTOMERS = new List<Customer>();

                public InMemoryCustomerRepository()
                {
                        if (CUSTOMERS.Count == 0)
                        {
                                this.SeedData();
                        }
                }

                /// <inheritdoc/>
                public Task<Customer?> GetByIdAsync(Guid id)
                {
                        Customer? customer = CUSTOMERS.FirstOrDefault(predicate: c => c.Id.Value == id);
                        return Task.FromResult(result: customer);
                }

                /// <inheritdoc/>
                public Task<IReadOnlyList<Customer>> GetAllAsync()
                {
                        IReadOnlyList<Customer> readOnlyList = CUSTOMERS.AsReadOnly();
                        return Task.FromResult(result: readOnlyList);
                }

                /// <inheritdoc/>
                public Task<IReadOnlyList<Customer>> FindAsync(Specification<Customer> specification)
                {
                        ArgumentNullException.ThrowIfNull(argument: specification, paramName: nameof(specification));

                        // Convert the list to an IQueryable so we can dynamically chain the specification's expression trees
                        IQueryable<Customer> query = CUSTOMERS.AsQueryable();

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
                        ArgumentNullException.ThrowIfNull(argument: entity, paramName: nameof(entity));

                        CUSTOMERS.Add(item: entity);
                        return Task.FromResult(result: entity);
                }

                /// <inheritdoc/>
                public Task UpdateAsync(Customer entity)
                {
                        ArgumentNullException.ThrowIfNull(argument: entity, paramName: nameof(entity));

                        int index = CUSTOMERS.FindIndex(match: c => c.Id.Value == entity.Id.Value);

                        if (index != -1)
                        {
                                CUSTOMERS[index] = entity;
                        }
                        return Task.CompletedTask;
                }

                /// <inheritdoc/>
                public Task<bool> DeleteAsync(Guid id)
                {
                        int removedCount = CUSTOMERS.RemoveAll(match: c => c.Id.Value == id);
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

                        CUSTOMERS.Add(item: jonathan);

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

                        CUSTOMERS.Add(item: elizabeth);
                }
        }
}
