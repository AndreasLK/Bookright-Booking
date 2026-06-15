using Domain.Entities.Persons;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using Facade.Common.Dtos;
using UseCase.Customers;

namespace Facade.Customers
{
        /// <summary>
        /// Orchestrates customer-related operations between the UI layer and the Domain layer.
        /// Acts as a strict boundary, ensuring the presentation layer only interacts with DTOs 
        /// and never directly accesses Domain entities or Infrastructure.
        /// </summary>
        public class CustomerService
        {
                private readonly ICustomerRepository _customerRepository;
                private readonly RegisterCustomerUseCase _registerCustomerUseCase;

                /// <summary>
                /// Initializes a new instance of the <see cref="CustomerService"/> class.
                /// </summary>
                /// <param name="customerRepository">The data access implementation for customer entities.</param>
                /// <param name="registerCustomerUseCase"></param>
                /// <exception cref="ArgumentNullException">Thrown when the injected repository is null.</exception>
                public CustomerService(ICustomerRepository customerRepository, RegisterCustomerUseCase registerCustomerUseCase)

                {
                        if (customerRepository is null)
                        {
                                throw new ArgumentNullException(paramName: nameof(customerRepository));
                        }
                        this._customerRepository = customerRepository;
                        this._registerCustomerUseCase = registerCustomerUseCase;
                }

                /// <summary>
                /// Retrieves a summarized list of all customers, optionally filtered by a search term.
                /// </summary>
                /// <param name="searchTerm">Optional text to filter customers by name or email.</param>
                /// <returns>A collection of customer summaries.</returns>
                public async Task<IEnumerable<CustomerSummaryDto>> SearchCustomersAsync(string searchTerm = "")
                {
                        IEnumerable<Customer> domainCustomers = await this._customerRepository.GetAllAsync();

                        // Map Domain Entities -> DTOs
                        var dtos = domainCustomers.Select(selector: c => new CustomerSummaryDto
                        {
                                Id = c.Id.Value,
                                LegalFirstName = c.LegalFirstName,
                                LegalLastName = c.LegalLastName,
                                PreferredFirstName = c.PreferredFirstName,
                                Pronouns = c.Pronouns,
                                PhoneNumber = c.PhoneNumber.Value,
                                Email = c.Email.Value,
                                LoyaltyLevel = c.Loyality.ToString()
                        });

                        if (!string.IsNullOrWhiteSpace(value: searchTerm))
                        {
                                string lowerSearch = searchTerm.ToLower();
                                dtos = dtos.Where(predicate: d =>
                                        d.LegalFirstName.ToLower().Contains(value: lowerSearch) ||
                                        d.LegalLastName.ToLower().Contains(value: lowerSearch));
                        }

                        return dtos.ToList();
                }

                /// <summary>
                /// Retrieves the full, editable details of a specific customer.
                /// </summary>
                /// <param name="id">The unique identifier of the customer.</param>
                /// <returns>The customer details DTO, or null if not found.</returns>
                public async Task<CustomerDetailsDto?> GetCustomerDetailsAsync(Guid id)
                {
                        Customer? customer = await this._customerRepository.GetByIdAsync(id: id);

                        if (customer is null)
                        {
                                return null;
                        }

                        // Map Domain Entity -> DTO
                        return new CustomerDetailsDto
                        {
                                Id = customer.Id.Value,
                                LegalFirstName = customer.LegalFirstName,
                                LegalLastName = customer.LegalLastName,
                                PreferredFirstName = customer.PreferredFirstName,
                                Pronouns = customer.Pronouns,
                                Gender = (GenderDto)customer.Gender,
                                Email = customer.Email.Value,
                                PhoneNumber = customer.PhoneNumber.Value,
                                DateOfBirth = customer.DateOfBirth,
                                LoyaltyLevel = customer.Loyality.ToString(),
                                SygsikringDanmarkMember = customer.SygsikringDanmarkMember,
                                ImportantNote = customer.ImportantNote
                        };
                }

                /// <summary>
                /// Updates an existing customer with data provided from the UI.
                /// </summary>
                /// <param name="dto">The updated customer details.</param>
                /// <exception cref="ArgumentNullException">Thrown if the provided DTO is null.</exception>
                /// <exception cref="InvalidOperationException">Thrown if the customer does not exist in the database.</exception>
                public async Task UpdateCustomerAsync(CustomerDetailsDto dto)
                {
                        ArgumentNullException.ThrowIfNull(argument: dto, paramName: nameof(dto));

                        Customer? existingCustomer = await this._customerRepository.GetByIdAsync(id: dto.Id);

                        if (existingCustomer is null)
                        {
                                throw new InvalidOperationException(message: $"Customer with ID {dto.Id} was not found.");
                        }

                        PersonDetails updatedDetails = new PersonDetails(
                                LegalFirstName: dto.LegalFirstName,
                                LegalLastName: dto.LegalLastName,
                                Pronouns: dto.Pronouns,
                                DateOfBirth: dto.DateOfBirth,
                                PhoneNumber: new PhoneNumber(value: dto.PhoneNumber),
                                Email: new EmailAddress(value: dto.Email),
                                Gender: (Domain.Enums.Gender)dto.Gender, // Fully qualified to avoid conflict with DTO enum
                                PreferredFirstName: dto.PreferredFirstName,
                                PreferredLastName: null // Or add to DTO if needed
                        );

                        existingCustomer.UpdateCustomerProfile(
                                details: updatedDetails,
                                personalNote: null, // Add to DTO if you want to edit it
                                importantNote: dto.ImportantNote,
                                sygsikringDanmarkMember: dto.SygsikringDanmarkMember
                        );

                        await this._customerRepository.UpdateAsync(entity: existingCustomer);
                }

                /// <summary>
                /// Registers a new customer in the system.
                /// Delegates to RegisterCustomerUseCase which handles validation and persistence.
                /// </summary>
                /// <param name="dto">The new customer details from the UI.</param>
                /// <returns>The new customer's ID if successful, or an error message if not.</returns>
                public async Task<RegisterCustomerResult> RegisterCustomerAsync(CustomerDetailsDto dto)
                {
                        ArgumentNullException.ThrowIfNull(argument: dto, paramName: nameof(dto));

                        var command = new RegisterCustomerCommand(
                                LegalFirstName: dto.LegalFirstName,
                                LegalLastName: dto.LegalLastName,
                                Pronouns: dto.Pronouns,
                                DateOfBirth: dto.DateOfBirth,
                                PhoneNumber: dto.PhoneNumber,
                                Email: dto.Email,
                                Gender: (Domain.Enums.Gender)dto.Gender,
                                PersonalNote: null,
                                ImportantNote: dto.ImportantNote
                        );

                        return await this._registerCustomerUseCase.ExecuteAsync(command);
                }

        }
}
