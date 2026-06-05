using Domain.Enums;

namespace Domain.Value_Objects
{
        public record PersonDetails(
            string LegalFirstName,
            string LegalLastName,
            string Pronouns,
            DateOnly DateOfBirth,
            PhoneNumber PhoneNumber,
            EmailAddress Email,
            Gender Gender,
            string? PreferredFirstName = null,
            string? PreferredLastName = null);
}
