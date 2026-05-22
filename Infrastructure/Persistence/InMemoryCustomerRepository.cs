using Domain.Entities.Persons;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Infrastructure.Persistence
{
        /// <summary>
        /// A fully functional in-memory implementation of the customer repository.
        /// Evaluates specifications, queries, and state changes locally without a database.
        /// </summary>
        public class InMemoryCustomerRepository : ICustomerRepository
        {
                private readonly List<Customer> _customers = new List<Customer>();

                /// <summary>
                /// Initializes a new instance of the <see cref="InMemoryCustomerRepository"/> class.
                /// Seeds initial mock data if the collection is empty.
                /// </summary>
                public InMemoryCustomerRepository()
                {
                        if (this._customers.Count == 0)
                        {
                                this.SeedData();
                        }
                }

                /// <inheritdoc/>
                public Task<Customer?> GetByIdAsync(Guid id)
                {
                        Customer? customer = this._customers.FirstOrDefault(predicate: c => c.Id.Value == id);
                        return Task.FromResult(result: customer);
                }

                /// <inheritdoc/>
                public Task<IReadOnlyList<Customer>> GetAllAsync()
                {
                        IReadOnlyList<Customer> readOnlyList = this._customers.AsReadOnly();
                        return Task.FromResult(result: readOnlyList);
                }

                /// <inheritdoc/>
                public Task<IReadOnlyList<Customer>> FindAsync(Specification<Customer> specification)
                {
                        ArgumentNullException.ThrowIfNull(argument: specification, paramName: nameof(specification));

                        // Convert the list to an IQueryable so we can dynamically chain the specification's expression trees
                        IQueryable<Customer> query = this._customers.AsQueryable();

                        // 1. Apply the Filtering Rule
                        query = query.Where(predicate: specification.ToExpression());

                        // 2. Apply Sorting
                        if (specification.OrderBy is not null)
                        {
                                query = query.OrderBy(keySelector: specification.OrderBy);
                        }
                        else if (specification.OrderByDescending is not null)
                        {
                                query = query.OrderByDescending(keySelector: specification.OrderByDescending);
                        }

                        // 3. Apply Pagination / Record Limits
                        if (specification.Take.HasValue)
                        {
                                query = query.Take(count: specification.Take.Value);
                        }

                        IReadOnlyList<Customer> results = query.ToList().AsReadOnly();
                        return Task.FromResult(result: results);
                }

                /// <inheritdoc/>
                public Task<Customer> AddAsync(Customer entity)
                {
                        ArgumentNullException.ThrowIfNull(argument: entity, paramName: nameof(entity));

                        this._customers.Add(item: entity);
                        return Task.FromResult(result: entity);
                }

                /// <inheritdoc/>
                public Task UpdateAsync(Customer entity)
                {
                        ArgumentNullException.ThrowIfNull(argument: entity, paramName: nameof(entity));

                        int index = this._customers.FindIndex(match: c => c.Id.Value == entity.Id.Value);

                        if (index != -1)
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
                /// Populates the initial memory state with test customers for development and UI testing.
                /// Data is migrated from the UI mock components into strict Domain entities.
                /// </summary>
                private void SeedData()
                {
                        // 1. Migrate Jonathan Doe (From CustomerSearch and CustomerDetails UI)
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

                        // 2. Migrate Elizabeth Windsor (From CustomerSearch UI)
                        PersonDetails elizabethDetails = new PersonDetails(
                                LegalFirstName: "Elizabeth",
                                LegalLastName: "Windsor",
                                Pronouns: "She/They",
                                DateOfBirth: new DateOnly(year: 1926, month: 4, day: 21), // Placeholder DOB
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
