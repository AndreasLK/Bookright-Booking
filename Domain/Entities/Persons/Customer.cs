
using Domain.Entities.Persons;
using Domain.Enums;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Entities.People
{
        /// <inheritdoc />
        public abstract class Customer : Person
        {
                /// <summary>
                /// Unique customer identifier.
                /// </summary>
                public CustomerId Id { get; init; }

                /// <summary>
                /// Loyalty tier determined by purchase history.
                /// </summary>
                public LoyalityLevel Loyality { get; }

                /// <summary>
                /// General health and administrative notes.
                /// </summary>
                public string? PersonalNote { get; private set; }

                /// <summary>
                /// Critical medical alerts, such as allergies or acute conditions.
                /// </summary>
                public string? ImportantNote { get; private set; }

                /// <summary>
                /// ID of the practitioner the customer prefers to see.
                /// </summary>
                public Guid? PreferredPratitionerId { get; private set; }

                /// <summary>
                /// Preferred gender of the treating practitioner.
                /// </summary>
                public Gender? PreferredGender { get; private set; }

                /// <summary>
                /// Genders the customer explicitly excludes for treatments.
                /// </summary>
                public List<Gender> UnwantedGenders = new List<Gender>();

                /// <summary>
                /// Membership status in 'Sygeforsikringen "danmark"'.
                /// </summary>
                public bool SygsikringDanmarkMember { get; private set; }


                public Customer(
                        CustomerId id,
                        LoyalityLevel loyality,
                        string? personalNote,
                        string? importantNote,
                        Guid? preferredPratitionerId,
                        Gender? preferredGender,
                        bool sygsikringDanmarkMember,
                        string legalFirstName,
                        string legalLastName,
                        string pronouns,
                        DateOnly dateOfBirth,
                        PhoneNumber phoneNumber,
                        EmailAddress email,
                        Gender gender,
                        string? preferredFirstName = null,
                        string? preferredLastName = null) : base(
                                legalFirstName: legalFirstName,
                                legalLastName: legalLastName,
                                pronouns: pronouns,
                                dateOfBirth: dateOfBirth,
                                phoneNumber: phoneNumber,
                                email: email,
                                gender: gender,
                                preferredFirstName: preferredFirstName,
                                preferredLastName: preferredLastName
                              )
                {
                        this.Id = id;
                        this.Loyality = loyality;
                        this.PersonalNote = personalNote;
                        this.ImportantNote = importantNote;
                        this.PreferredPratitionerId = preferredPratitionerId;
                        this.PreferredGender = preferredGender;
                        this.SygsikringDanmarkMember = sygsikringDanmarkMember;
                }
        }
}
