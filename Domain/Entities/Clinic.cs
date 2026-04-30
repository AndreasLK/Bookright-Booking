using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;
using Domain.Value_Objects;
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
                private readonly List<Guid> RoomIds = new List<Guid>();


                public Clinic(ClinicId id,
                                string Name,
                                Address Address,
                                PhoneNumber PhoneNumber,
                                EmailAddress Email,
                                List<Guid> RoomIds)

                {
                        if (id == null)
                                throw new ArgumentNullException(nameof(id));
                        if (Name == null)
                                throw new ArgumentNullException(nameof(Name));
                        if (Address == null)
                                throw new ArgumentNullException(nameof(Address));
                        if (PhoneNumber == null)
                                throw new ArgumentNullException(nameof(PhoneNumber));
                        if (Email == null)
                                throw new ArgumentNullException(nameof(Email));
                        if (RoomIds == null)
                                throw new ArgumentNullException(nameof(RoomIds));

                        this.Id = id;
                        this.Name = Name;
                        this.Address = Address;
                        this.PhoneNumber = PhoneNumber;
                        this.Email = Email;
                        this.RoomIds = RoomIds;

                }

        }
}
