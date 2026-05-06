using Facade.Common.Attributes;
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

                [Searchable]
                public string LegalFirstName { get; set; }

                [Searchable]
                public string LegalLastName { get; set; }

                [Searchable]
                public string? PreferredFirstName { get; set; }

                [Searchable]
                public string? PreferredLastName { get; set; }
                public string Pronouns { get; set; }

                [Searchable]
                public string PhoneNumber { get; set; }

                [Searchable]
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
