using Domain.Enums;

namespace UseCase.Customers
{
        public record RegisterCustomerCommand(
         string LegalFirstName,
         string LegalLastName,
         string Pronouns,
         DateOnly DateOfBirth,
         string PhoneNumber,
         string Email,
         Gender Gender,
         string? PersonalNote,
         string? ImportantNote);

        public record RegisterCustomerResult(
                bool Success,
                Guid? CustomerId,
                string? ErrorMessage);
}
