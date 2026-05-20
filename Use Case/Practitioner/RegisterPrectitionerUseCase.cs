using System;
using System.Collections.Generic;
using System.Text;
using Domain.Interfaces.Repositories;

namespace Use_Case.Practitioner
{
        public class RegisterPractitionerUseCase
        {
                private readonly IPractitionerRepository _practitioner;

                public RegisterPractitionerUseCase(IPractitionerRepository practitioner)
                {
                        this._practitioner = practitioner;
                }

                public async Task<RegisterPractitionerResult> ExecuteAsync(RegisterPractitionerCommand cmd) 



        }
}
