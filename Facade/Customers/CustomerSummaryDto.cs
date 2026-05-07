using Facade.Common.Attributes;

namespace Facade.Customers
{
        /// <summary>
        /// Specific summary for the receptionist search list. 
        /// Includes inclusive greeting data and contact info.
        /// </summary>
        public class CustomerSummaryDto
        {
                /// <summary>
                /// Unique customer identifier.
                /// </summary>
                public Guid Id { get; set; }

                /// <summary>
                /// Official legal first name.
                /// </summary>
                [Searchable]
                public string LegalFirstName { get; set; }

                /// <summary>
                /// Official legal last name.
                /// </summary>
                [Searchable]
                public string LegalLastName { get; set; }

                /// <summary>
                /// Optional preferred first name.
                /// </summary>
                [Searchable]
                public string? PreferredFirstName { get; set; }

                /// <summary>
                /// Optional preferred last name.
                /// </summary>
                [Searchable]
                public string? PreferredLastName { get; set; }

                /// <summary>
                /// Customer's personal pronouns.
                /// </summary>
                public string Pronouns { get; set; }

                /// <summary>
                /// Primary phone number.
                /// </summary>
                [Searchable]
                public string PhoneNumber { get; set; }

                /// <summary>
                /// Primary email address.
                /// </summary>
                [Searchable]
                public string Email { get; set; }

                /// <summary>
                /// Dynamically calculated loyalty tier.
                /// </summary>
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
