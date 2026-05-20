using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Enums;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Use_Case.Practitioner
{
        public record RegisterPractitionerCommand(
                string LegalFirstName,
                string LegalLastName,
                string Alias,
                DateOnly DateOfBirth,
                String Email,
                String PhoneNumber,
                Gender Gender,
                Certificate Certificate);

        public record RegisterPractitionerResult(
                bool Success,
                Guid? CustomerId,
                string? ErrorMessage);
       
}
