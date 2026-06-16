using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Persons;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using Use_Case.Practitioners; // Updated to match your Use Case namespace structure

namespace Facade.Practitioners
{
        /// <summary>
        /// Orchestrates practitioner operations between the UI and domain layers.
        /// </summary>
        public class PractitionerService
        {
                private readonly IPractitionerRepository _practitionerRepository;
                private readonly RegisterPractitionerUseCase _registerPractitionerUseCase;

                /// <summary>
                /// Initializes a new instance of the <see cref="PractitionerService"/> class.
                /// </summary>
                public PractitionerService(IPractitionerRepository practitionerRepository, RegisterPractitionerUseCase registerPractitionerUseCase)
                {
                        ArgumentNullException.ThrowIfNull(argument: practitionerRepository, paramName: nameof(practitionerRepository));
                        ArgumentNullException.ThrowIfNull(argument: registerPractitionerUseCase, paramName: nameof(registerPractitionerUseCase));

                        this._practitionerRepository = practitionerRepository;
                        this._registerPractitionerUseCase = registerPractitionerUseCase;
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
                                FirstName = p.Details.PreferredFirstName ?? p.Details.LegalFirstName ?? string.Empty,
                                LastName = p.Details.PreferredLastName ?? p.Details.LegalLastName ?? string.Empty,
                                Email = p.Details.Email.Value,
                                PhoneNumber = p.Details.PhoneNumber.Value
                        }).ToList();
                }

                /// <summary>
                /// Retrieves a single record by its identifier.
                /// </summary>
                public async Task<PractitionerSummaryDto?> GetPractitionerByIdAsync(Guid id)
                {
                        Practitioner? practitioner = await this._practitionerRepository.GetByIdAsync(id: id);

                        if (practitioner is null)
                        {
                                return null;
                        }

                        return new PractitionerSummaryDto
                        {
                                Id = practitioner.Id.Value,
                                Alias = practitioner.Alias,
                                FirstName = practitioner.Details.PreferredFirstName ?? practitioner.Details.LegalFirstName ?? string.Empty,
                                LastName = practitioner.Details.PreferredLastName ?? practitioner.Details.LegalLastName ?? string.Empty,
                                Email = practitioner.Details.Email.Value,
                                PhoneNumber = practitioner.Details.PhoneNumber.Value
                        };
                }

                /// <summary>
                /// Updates the basic details and contact information of a practitioner.
                /// </summary>
                public async Task UpdatePractitionerAsync(PractitionerSummaryDto model)
                {
                        ArgumentNullException.ThrowIfNull(argument: model, paramName: nameof(model));

                        Practitioner? practitioner = await this._practitionerRepository.GetByIdAsync(id: model.Id);

                        if (practitioner is null)
                        {
                                throw new InvalidOperationException(message: $"Practitioner with ID '{model.Id}' was not found.");
                        }

                        EmailAddress updatedEmail = new EmailAddress(value: model.Email);
                        PhoneNumber updatedPhone = new PhoneNumber(value: model.PhoneNumber);

                        PersonDetails updatedDetails = practitioner.Details with
                        {
                                Email = updatedEmail,
                                PhoneNumber = updatedPhone,
                                PreferredFirstName = model.FirstName,
                                PreferredLastName = model.LastName
                        };

                        practitioner.UpdateAlias(newAlias: model.Alias);
                        practitioner.UpdateDetails(newDetails: updatedDetails);

                        await this._practitionerRepository.UpdateAsync(entity: practitioner);
                }

                /// <summary>
                /// Registers a new practitioner in the system without certificates.
                /// </summary>
                public async Task<RegisterPractitionerResult> RegisterPractitionerAsync(PractitionerSummaryDto model)
                {
                        ArgumentNullException.ThrowIfNull(argument: model, paramName: nameof(model));

                        RegisterPractitionerCommand command = new RegisterPractitionerCommand(
                                LegalFirstName: model.FirstName,
                                LegalLastName: model.LastName,
                                Pronouns: string.Empty,
                                Alias: model.Alias,
                                DateOfBirth: DateOnly.FromDateTime(dateTime: DateTime.Now),
                                Email: model.Email,
                                PhoneNumber: model.PhoneNumber,
                                Gender: Domain.Enums.Gender.PreferNotToSay,
                                Certificates: new List<Certificate>().AsReadOnly()
                        );

                        return await this._registerPractitionerUseCase.ExecuteAsync(cmd: command);
                }
        }
}
