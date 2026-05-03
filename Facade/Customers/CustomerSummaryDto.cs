using System.Security.Cryptography.X509Certificates;

namespace Facade.Customers
{
        /// <summary>
        /// Specific summary for the receptionist search list. 
        /// Includes inclusive greeting data and contact info.
        /// </summary>
        public class CustomerSummaryDto
        {
                public Guid Id { get; set; }
                public string LegalFirstName { get; set; }
                public string LegalLastName { get; set; }
                public string? PreferredFirstName { get; set; }
                public string? PreferredLastName { get; set; }
                public string Pronouns { get; set; }
                public string PhoneNumber { get; set; }
                public string Email { get; set; }
                public string LoyaltyLevel { get; set; }

                /// <summary>
                /// Logic to determine the correct name to use when greeting the customer.
                /// </summary>
                public string GreetingName =>
                        string.IsNullOrEmpty(
                                value: this.PreferredFirstName)
                                ? this.LegalFirstName : this.PreferredFirstName;

        }
}
