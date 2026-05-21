
using Domain.Entities;
using Domain.Entities.Persons;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;



namespace UseCase.Practitioners
{
        public class RegisterPractitionerUseCase
        {
                private readonly IPractitionerRepository _practitioner;

                public RegisterPractitionerUseCase(IPractitionerRepository practitioner)
                {
                        this._practitioner = practitioner;
                }

                public async Task<RegisterPractitionerResult> ExecuteAsync(RegisterPractitionerCommand cmd)
                {

                        try
                        {
                                var details = new PersonDetails(LegalFirstName: cmd.LegalFirstName,
                                        LegalLastName: cmd.LegalLastName,
                                        Pronouns: cmd.Pronouns,
                                        DateOfBirth: cmd.DateOfBirth,
                                        Email: new EmailAddress(cmd.Email),
                                        PhoneNumber: new PhoneNumber(cmd.PhoneNumber),
                                        Gender: cmd.Gender);



                                var practitioner = new Practitioner(id: new PractitionerId(Guid.NewGuid()),
                                                                  alias: cmd.Alias,
                                                                  details: details);


                                return new RegisterPractitionerResult(
                                        Success: true,
                                        PractitionerId: practitioner.Id.Value,
                                        ErrorMessage: null);






                        }
                        catch (Exception ex)
                        {
                                return new RegisterPractitionerResult(
                                        Success: false,
                                        PractitionerId: null,
                                        ErrorMessage: ex.Message);
                        }

                }



        }
}
