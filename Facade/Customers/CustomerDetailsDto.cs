using Facade.Common.Dtos;

namespace Facade.Customers
{
        /// <summary>
        /// Comprehensive data transfer object containing all customer information.
        /// Used for detailed views and full profile displays.
        /// </summary>
        public class CustomerDetailsDto
        {
                /// <summary>
                /// Unique customer identifier.
                /// </summary>
                public Guid Id { get; set; }

                /// <summary>
                /// Official first name.
                /// </summary>
                public string LegalFirstName { get; set; } = string.Empty;

                /// <summary>
                /// Official last name.
                /// </summary>
                public string LegalLastName { get; set; } = string.Empty;

                /// <summary>
                /// Optional preferred first name.
                /// </summary>
                public string? PreferredFirstName { get; set; }

                /// <summary>
                /// Optional preferred last name.
                /// </summary>
                public string? PreferredLastName { get; set; }

                /// <summary>
                /// Personal pronouns.
                /// </summary>
                public string Pronouns { get; set; } = string.Empty;

                /// <summary>
                /// Date of birth.
                /// </summary>
                public DateOnly DateOfBirth { get; set; }

                /// <summary>
                /// Primary phone number, flattened from Domain Value Object.
                /// </summary>
                public string PhoneNumber { get; set; } = string.Empty;

                /// <summary>
                /// Primary email address, flattened from Domain Value Object.
                /// </summary>
                public string Email { get; set; } = string.Empty;

                /// <summary>
                /// Gender identity, represented as a string.
                /// </summary>
                public string Gender { get; set; } = string.Empty;

                /// <summary>
                /// Loyalty tier determined by purchase history.
                /// </summary>
                public string LoyaltyLevel { get; set; } = string.Empty;

                /// <summary>
                /// General health and administrative notes.
                /// </summary>
                public string? PersonalNote { get; set; }

                /// <summary>
                /// Critical medical alerts, such as allergies or acute conditions.
                /// </summary>
                public string? ImportantNote { get; set; }

                /// <summary>
                /// ID of the practitioner the customer prefers to see.
                /// </summary>
                public PractitionerLookupDto? PreferredPratitionerId { get; set; }

                /// <summary>
                /// Preferred gender of the treating practitioner.
                /// </summary>
                public string? PreferredGender { get; set; }

                /// <summary>
                /// Genders the customer explicitly excludes for treatments.
                /// </summary>
                public List<string> UnwantedGenders { get; set; } = new List<string>();

                /// <summary>
                /// Membership status in 'Sygeforsikringen "danmark"'.
                /// </summary>
                public bool SygsikringDanmarkMember { get; set; }

                /// <summary>
                /// Logic to determine the correct name to use when greeting the customer.
                /// </summary>
                public string GreetingName =>
                    string.IsNullOrEmpty(value: this.PreferredFirstName)
                        ? this.LegalFirstName
                        : this.PreferredFirstName;
        }
}
