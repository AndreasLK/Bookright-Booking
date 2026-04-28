
using Domain.Entities.Persons;
using Domain.Enums;

namespace Domain.Entities.People
{
        public class Customer : Person
        {
                public LoyalityLevel Loyality { get; }
                public string? PersonalNote { get; private set; }
                public string? ImportantNote { get; private set; }

                public Guid? PreferredPratitionerId { get; private set; }

                public Gender? PreferredGender { get; private set; }

                public List<Gender> UnwantedGenders = new List<Gender>();

                public bool SygsikringDanmarkMemeber { get; private set; }
        }
}
