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
            string? PreferredLastName = null)
        {
                public PersonDetails(string LegalFirstName, string LegalLastName, string Pronouns, DateOnly DateOfBirth, PhoneNumber PhoneNumber, EmailAddress Email, Gender Gender)
                {
                        this.LegalFirstName = LegalFirstName;
                        this.LegalLastName = LegalLastName;
                        this.Pronouns = Pronouns;
                        this.DateOfBirth = DateOfBirth;
                        this.PhoneNumber = PhoneNumber;
                        this.Email = Email;
                        this.Gender = Gender;
                }
        }
}
