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

                protected Person(PersonDetails details)
                {
                        //Guard clauses
                        if (details is null) throw new ArgumentNullException(nameof(details));

                        if (string.IsNullOrWhiteSpace(details.LegalFirstName))
                        {
                                throw new ArgumentException(
                                        message: "Legal first name cannot be blank",
                                        paramName: nameof(details.LegalFirstName)
                                        );
                        }
                        if (string.IsNullOrWhiteSpace(details.LegalLastName))
                        {
                                throw new ArgumentException(
                                        message: "Legal last name cannot be blank",
                                        paramName: nameof(details.LegalLastName)
                                        );
                        }
                        if (string.IsNullOrWhiteSpace(details.Pronouns))
                        {
                                throw new ArgumentException(
                                        message: "Person must have pronouns",
                                        paramName: nameof(details.Pronouns)
                                        );
                        }
                        if (details.DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
                        {
                                throw new ArgumentException(
                                        message: "Date of birth can't be in the future",
                                        paramName: nameof(details.DateOfBirth));
                        }

                        if (details.DateOfBirth < new DateOnly(1990, 1, 1))
                        {
                                throw new ArgumentException(
                                message: "Date of Birth is unrealistic",
                                paramName: nameof(details.DateOfBirth));
                        }
                        if (string.IsNullOrWhiteSpace(details.PhoneNumber))
                        {
                                throw new ArgumentException(
                                message: "PhoneNumber can't be empty",
                                paramName: nameof(details.PhoneNumber));
                        }




                        this.LegalFirstName = details.LegalFirstName;
                        this.LegalLastName = details.LegalLastName;
                        this.PreferredFirstName = details.PreferredFirstName;
                        this.PreferredLastName = details.PreferredLastName;
                        this.Pronouns = details.Pronouns;
                        this.DateOfBirth = details.DateOfBirth;
                        this.PhoneNumber = details.PhoneNumber;
                        this.Email = details.Email;
                        this.Gender = details.Gender;
                }
        }
}
