using Domain.Enums;
using Domain.Value_Objects;

namespace Domain.Entities.Persons
{
        public abstract class Person
        {
                public Guid Id { get; private set; }
                public string LegalFirstName { get; private set; }
                public string LegalLastName { get; private set; }

                public string? PreferredFirstName { get; private set; } 

                public string? PreferredLastName { get; private set; }
                public string Pronouns { get; private set; }
                public DateOnly Birthday { get; private set; }
                public PhoneNumber PhoneNumber { get; private set; }
                public EmailAddress Email { get; private set; }
                public Gender Gender { get; private set; }

        }
}
