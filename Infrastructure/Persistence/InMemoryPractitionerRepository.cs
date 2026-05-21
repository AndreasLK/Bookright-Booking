using Domain.Entities.Persons;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Persistence
{
        /// <summary>
        /// A fake in-memory repository for Practitioners to satisfy the BookingService dependencies.
        /// </summary>
        public class InMemoryPractitionerRepository : IPractitionerRepository
        {
                private readonly List<Practitioner> _practitioners = new List<Practitioner>();

                /// <summary>
                /// Initializes a new instance of the <see cref="InMemoryPractitionerRepository"/> class.
                /// </summary>
                public InMemoryPractitionerRepository()
                {
                        this.SeedData();
                }

                /// <inheritdoc/>
                public Task<Practitioner?> GetByIdAsync(Guid id)
                {
                        Practitioner? practitioner = this._practitioners.FirstOrDefault(predicate: p => p.Id.Value == id);
                        return Task.FromResult(result: practitioner);
                }

                /// <inheritdoc/>
                public Task<IReadOnlyList<Practitioner>> GetAllAsync()
                {
                        IReadOnlyList<Practitioner> readOnlyList = this._practitioners.AsReadOnly();
                        return Task.FromResult(result: readOnlyList);
                }

                /// <inheritdoc/>
                public Task<IReadOnlyList<Practitioner>> FindAsync(Specification<Practitioner> specification)
                {
                        ArgumentNullException.ThrowIfNull(argument: specification, paramName: nameof(specification));

                        IQueryable<Practitioner> query = this._practitioners.AsQueryable();
                        query = query.Where(predicate: specification.ToExpression());

                        if (specification.OrderBy is not null)
                        {
                                query = query.OrderBy(keySelector: specification.OrderBy);
                        }
                        else if (specification.OrderByDescending is not null)
                        {
                                query = query.OrderByDescending(keySelector: specification.OrderByDescending);
                        }

                        if (specification.Skip.HasValue)
                        {
                                query = query.Skip(count: specification.Skip.Value);
                        }

                        if (specification.Take.HasValue)
                        {
                                query = query.Take(count: specification.Take.Value);
                        }

                        IReadOnlyList<Practitioner> results = query.ToList().AsReadOnly();
                        return Task.FromResult(result: results);
                }

                /// <inheritdoc/>
                public Task<Practitioner> AddAsync(Practitioner entity)
                {
                        ArgumentNullException.ThrowIfNull(argument: entity, paramName: nameof(entity));

                        this._practitioners.Add(item: entity);
                        return Task.FromResult(result: entity);
                }

                /// <inheritdoc/>
                public Task UpdateAsync(Practitioner entity)
                {
                        ArgumentNullException.ThrowIfNull(argument: entity, paramName: nameof(entity));

                        int index = this._practitioners.FindIndex(match: p => p.Id.Value == entity.Id.Value);

                        if (index != -1)
                        {
                                this._practitioners[index] = entity;
                        }

                        return Task.CompletedTask;
                }

                /// <inheritdoc/>
                public Task<bool> DeleteAsync(Guid id)
                {
                        int removedCount = this._practitioners.RemoveAll(match: p => p.Id.Value == id);
                        bool wasRemoved = removedCount > 0;

                        return Task.FromResult(result: wasRemoved);
                }

                /// <summary>
                /// Seeds a default practitioner that matches the GUID used in the InMemoryBookingRepository.
                /// </summary>
                private void SeedData()
                {
                        PersonDetails details = new PersonDetails(
                                LegalFirstName: "Sarah",
                                LegalLastName: "Smith",
                                Pronouns: "She/Her",
                                DateOfBirth: new DateOnly(year: 1985, month: 8, day: 22),
                                PhoneNumber: new PhoneNumber(value: "555-0199"),
                                Email: new EmailAddress(value: "sarah@downtownwellness.com"),
                                Gender: Gender.Woman,
                                PreferredFirstName: "Sarah",
                                PreferredLastName: null
                        );

                        // Note: This GUID matches the 'validPractitionerId' from InMemoryBookingRepository
                        Practitioner practitioner = new Practitioner(
                                id: new PractitionerId(Value: Guid.Parse(input: "B1111111-1111-1111-1111-111111111111")),
                                alias: "Dr. Smith",
                                details: details
                        );

                        this._practitioners.Add(item: practitioner);
                }
        }
}
