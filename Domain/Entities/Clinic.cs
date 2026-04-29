using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Entities
{
        /// <summary>
        /// Clinic where treaments are performened
        /// </summary>
        public class Clinic
        {
                /// <summary>
                /// Unique clinic identifier.
                /// </summary>
                public ClinicId Id { get; init; }

                /// <summary>
                /// Descriptive facility name.
                /// </summary>
                public string Name { get; private set; }

                /// <summary>
                /// Geographic location.
                /// </summary>
                public Address Address { get; private set; }

                /// <summary>
                /// Primary telephone contact.
                /// </summary>
                public PhoneNumber PhoneNumber { get; private set; }

                /// <summary>
                /// Primary email contact.
                /// </summary>
                public EmailAddress Email { get; private set; }

                /// <summary>
                /// Associated treatment room identifiers.
                /// </summary>
                public List<Guid> RoomIds = new List<Guid>();



        }
}
