
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

                public Customer (LoyalityLevel loyality,
                        string? personalNote,
                        string? importantNote,
                        Guid? preferredPratitionerId,
                        Gender? preferredGender, List<Gender> unwantedGenders, bool sygsikringDanmarkMemeber)
                {
                        this.Loyality = loyality;
                        this.PersonalNote = personalNote;
                        this.ImportantNote = importantNote;
                        this.PreferredPratitionerId = preferredPratitionerId;
                        this.PreferredGender = preferredGender;
                        this.UnwantedGenders = unwantedGenders;
                        this.SygsikringDanmarkMemeber = sygsikringDanmarkMemeber;
                }
        }
}
