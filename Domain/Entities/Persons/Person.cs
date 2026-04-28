using Domain.Value_Objects;

namespace Domain.Entities.Persons
{
        public abstract class Person
        {
                public Guid Id { get; private set; }
                public string FirstName { get; private set; }
                public string LastName { get; private set; }
                public DateOnly Birthday { get; private set; }
                public PhoneNumber PhoneNumber { get; private set; }
                public EmailAddress Email { get; private set; }

        }
}
