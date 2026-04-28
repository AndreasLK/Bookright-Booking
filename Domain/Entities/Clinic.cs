using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;
using Domain.Value_Objects;
using Domain.Value_Objects;

namespace Domain.Entities
{
        public class Clinic
        {
                public Guid Id { get; init; }

                public string Name { get; private set; }

                public Address Address { get; private set; }

                public PhoneNumber PhoneNumber { get; private set; }

                public EmailAddress Email { get; private set; }

                public List<Guid> RoomIds = new List<Guid>();



        }
}
