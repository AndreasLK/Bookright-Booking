using Domain.Entities;
using Domain.Enums;

namespace Use_Case.Practitioners
{
        public record RegisterPractitionerCommand(
                string LegalFirstName,
                string LegalLastName,
                string Pronouns,
                string Alias,
                DateOnly DateOfBirth,
                string Email,
                string PhoneNumber,
                Gender Gender,
                IReadOnlyList<Certificate> Certificates);

        public record RegisterPractitionerResult(
                bool Success,
                Guid? PractitionerId,
                string? ErrorMessage);
}
