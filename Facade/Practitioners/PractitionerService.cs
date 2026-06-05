using Domain.Entities.Persons;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Facade.Practitioners
{
        /// <summary>
        /// Orchestrates practitioner operations between the UI and domain layers.
        /// </summary>
        public class PractitionerService
        {
                private readonly IPractitionerRepository _practitionerRepository;

                /// <summary>
                /// Initializes a new instance of the <see cref="PractitionerService"/> class.
                /// </summary>
                /// <param name="practitionerRepository">Data store for practitioners.</param>
                public PractitionerService(IPractitionerRepository practitionerRepository)
                {
                        ArgumentNullException.ThrowIfNull(argument: practitionerRepository, paramName: nameof(practitionerRepository));
                        this._practitionerRepository = practitionerRepository;
                }

                /// <summary>
                /// Retrieves all practitioners mapped to summary data transfer objects.
                /// </summary>
                public async Task<IEnumerable<PractitionerSummaryDto>> GetPractitionersAsync()
                {
                        IReadOnlyList<Practitioner> practitioners = await this._practitionerRepository.GetAllAsync();

                        return practitioners.Select(selector: p => new PractitionerSummaryDto
                        {
                                Id = p.Id.Value,
                                Alias = p.Alias,
                                FirstName = p.Details.PreferredFirstName ?? p.Details.LegalFirstName,
                                LastName = p.Details.PreferredLastName ?? p.Details.LegalLastName,
                                Email = p.Details.Email.Value,
                                PhoneNumber = p.Details.PhoneNumber.Value
                        }).ToList();
                }

                /// <summary>
                /// Retrieves a single record by its identifier.
                /// </summary>
                /// <param name="id">The unique identifier.</param>
                public async Task<PractitionerSummaryDto?> GetPractitionerByIdAsync(Guid id)
                {
                        Practitioner? practitioner = await this._practitionerRepository.GetByIdAsync(id: id);

                        if (practitioner == null)
                        {
                                return null;
                        }

                        return new PractitionerSummaryDto
                        {
                                Id = practitioner.Id.Value,
                                Alias = practitioner.Alias,
                                FirstName = practitioner.Details.PreferredFirstName ?? practitioner.Details.LegalFirstName,
                                LastName = practitioner.Details.PreferredLastName ?? practitioner.Details.LegalLastName,
                                Email = practitioner.Details.Email.Value,
                                PhoneNumber = practitioner.Details.PhoneNumber.Value
                        };
                }

                /// <summary>
                /// Updates the basic details and contact information of a practitioner.
                /// </summary>
                /// <param name="model">The mutated summary object containing the updates.</param>
                public async Task UpdatePractitionerAsync(PractitionerSummaryDto model)
                {
                        ArgumentNullException.ThrowIfNull(argument: model, paramName: nameof(model));

                        Practitioner? practitioner = await this._practitionerRepository.GetByIdAsync(id: model.Id);

                        if (practitioner == null)
                        {
                                throw new InvalidOperationException(message: $"Practitioner with ID '{model.Id}' was not found.");
                        }

                        // Rebuild value objects safely
                        EmailAddress updatedEmail = new EmailAddress(value: model.Email);
                        PhoneNumber updatedPhone = new PhoneNumber(value: model.PhoneNumber);

                        // Use C# 'with' expression clone on the state projection block
                        PersonDetails updatedDetails = practitioner.Details with
                        {
                                Email = updatedEmail,
                                PhoneNumber = updatedPhone,
                                PreferredFirstName = model.FirstName,
                                PreferredLastName = model.LastName
                        };

                        // Push mutations into your updated domain methods
                        practitioner.UpdateAlias(newAlias: model.Alias);
                        practitioner.UpdateDetails(newDetails: updatedDetails);

                        await this._practitionerRepository.UpdateAsync(entity: practitioner);
                }
        }
}
