using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities.Persons;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using Domain.Specifications.Customers;

namespace UseCase.Customers
{
        public class RegisterCustomerUseCase
        {
                private readonly ICustomerRepository _customers;

                public RegisterCustomerUseCase(ICustomerRepository customers)
                {
                        this._customers = customers;
                }

                public async Task<RegisterCustomerResult> ExecuteAsync(RegisterCustomerCommand cmd)
                {

                        try
                        {
                                var emailSpec = new CustomerByEmailSpecification(cmd.Email);
                                var existing = await this._customers.FindAsync(emailSpec);
                                if (existing.Any())
                                {
                                        return new RegisterCustomerResult(
                                                Success: false,
                                                CustomerId: null,
                                                ErrorMessage: "En kunde med denne email findes allerede.");
                                }

                                var details = new PersonDetails(LegalFirstName: cmd.LegalFirstName,
                                                                LegalLastName: cmd.LegalLastName,
                                                                Pronouns: cmd.Pronouns,
                                                                DateOfBirth: cmd.DateOfBirth,
                                                                PhoneNumber: new PhoneNumber(cmd.PhoneNumber),
                                                                Email: new EmailAddress(cmd.Email),
                                                                Gender: cmd.Gender);

                                var customer = new Customer(
                                                                id: new CustomerId(Guid.NewGuid()),
                                                                personalNote: cmd.PersonalNote,
                                                                importantNote: cmd.ImportantNote,
                                                                preferredPratitionerId: null,
                                                                preferredGender: null,
                                                                sygsikringDanmarkMember: false,
                                                                details: details);

                                await this._customers.AddAsync(customer);

                                return new RegisterCustomerResult(Success: true,
                                                                  CustomerId: customer.Id.Value,
                                                                  ErrorMessage: null);

                        }
                        catch (Exception ex)
                        {

                                return new RegisterCustomerResult(Success: false,
                                                                   CustomerId: null,
                                                                   ErrorMessage: ex.Message);

                        }

                }
        }
}
