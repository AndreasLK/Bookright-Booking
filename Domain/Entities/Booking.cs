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

                public Booking(BookingId id,
                                Clinic clinic,
                                Practitioner practitioner,
                                Treatment treatment,
                                TimeSlot timeslot)

                {
                        if (id == null)
                                {
                                throw new ArgumentException(
                                message: "ID kan ikke være tomt")
                                {

                                };
                                this.Id = id;
                        this.Clinic = clinic;
                        this.Practitioner = practitioner;
                        this.Treatment = treatment;
                        this.Timeslot = timeslot;
                }
        }
}
