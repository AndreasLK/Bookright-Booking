using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Infrastructure.Persistence
{
        {
                {
                        }
                }

                {
                        return Task.CompletedTask;
                }

                /// <inheritdoc/>
                public Task<bool> DeleteAsync(Guid id)
                {
                        int removedCount = _customers.RemoveAll(match: c => c.Id.Value == id);
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

                        _customers.Add(item: jonathan);

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

                        _customers.Add(item: elizabeth);
                }
        }
}
