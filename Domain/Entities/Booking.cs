using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities.People;
using Domain.Value_Objects;

namespace Domain.Entities
{
        public class Booking
        {
                public BookingId Id { get; private set; }
                public Clinic Clinic { get; private set; }
                public Practitioner Practitioner { get; private set; }
                public Treatment Treatment { get; private set; }
                public TimeSlot Timeslot { get; private set; }
        }
}
