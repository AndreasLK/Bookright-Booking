using Domain.Enums;
using Domain.Value_Objects;

namespace Domain.Entities.Persons
{
        /// <summary>
        /// Individuals within the BookRight system.
        /// </summary>
        public abstract class Person
        {
                /// <summary>
                /// Official first name.
                /// </summary>
                public string LegalFirstName { get; private set; }

                /// <summary>
                /// Official last name.
                /// </summary>
                public string LegalLastName { get; private set; }

                /// <summary>
                /// Optional preferred first name.
                /// </summary>
                public string? PreferredFirstName { get; private set; }

                /// <summary>
                /// Optional preferred last name.
                /// </summary>
                public string? PreferredLastName { get; private set; }

                /// <summary>
                /// Personal pronouns.
                /// </summary>
                public string Pronouns { get; private set; }

                /// <summary>
                /// Date of birth.
                /// </summary>
                public DateOnly DateOfBirth { get; private set; }

                /// <summary>
                /// Primary Phone number.
                /// </summary>
                public PhoneNumber PhoneNumber { get; private set; }

                /// <summary>
                /// Primary email address.
                /// </summary>
                public EmailAddress Email { get; private set; }

                /// <summary>
                /// Gender identity.
                /// </summary>
                public Gender Gender { get; private set; }

                public Person(
                        string legalFirstName,
                        string legalLastName,
                        string pronouns,
                        DateOnly dateOfBirth,
                        PhoneNumber phoneNumber,
                        EmailAddress email,
                        Gender gender,
                        string? preferredFirstName = null,
                        string? preferredLastName = null)
                {

                        //Guard clauses
                        if (legalFirstName.IsWhiteSpace())
                        {
                                throw new ArgumentException(
                                        message: "Legal first name cannot be blank",
                                        paramName: nameof(legalFirstName)
                                        );
                        }
                        if (legalLastName.IsWhiteSpace())
                        {
                                throw new ArgumentException(
                                        message: "Legal last name cannot be blank",
                                        paramName: nameof(legalLastName)
                                        );
                        }
                        if (pronouns.IsWhiteSpace())
                        {
                                throw new ArgumentException(
                                        message: "Person must have pronouns",
                                        paramName: nameof(pronouns)
                                        );
                        }

                        this.LegalFirstName = legalFirstName;
                        this.LegalLastName = legalLastName;
                        this.PreferredFirstName = preferredFirstName;
                        this.PreferredLastName = preferredLastName;
                        this.Pronouns = pronouns;
                        this.DateOfBirth = dateOfBirth;
                        this.PhoneNumber = phoneNumber;
                        this.Email = email;
                        this.Gender = gender;
                }
        }
}
