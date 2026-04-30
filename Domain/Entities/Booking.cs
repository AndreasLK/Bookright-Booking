using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities.People;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Entities
{
        /// <summary>
        /// Scheduled appointment for a specific clinical service.
        /// </summary>
        public class Booking
        {
                /// <summary>
                /// Unique booking identifier.
                /// </summary>
                public BookingId Id { get; private set; }

                /// <summary>
                /// Associated clinic identifier.
                /// </summary>
                public ClinicId ClinicId { get; private set; }

                /// <summary>
                /// Assigned practitioner identifier.
                /// </summary>
                public PractitionerId PractitionerId { get; private set; }

                /// <summary>
                /// Selected treatment identifier.
                /// </summary>
                public TreatmentId TreatmentId { get; private set; }

                /// <summary>
                /// Allocated time window.
                /// </summary>
                public TimeSlot Timeslot { get; private set; }

                public Booking(BookingId id,
                                Clinic clinic,
                                Practitioner practitioner,
                                Treatment treatment,
                                TimeSlot timeslot)

                {
                        if (id == null)
                                throw new ArgumentNullException(nameof(id));

                        if (clinic == null)
                                throw new ArgumentNullException(nameof(clinic));

                        if (practitioner == null)
                                throw new ArgumentNullException(nameof(practitioner));

                        if (treatment == null)
                                throw new ArgumentNullException(nameof(treatment));

                        if (timeslot == null)
                                throw new ArgumentNullException(nameof(timeslot));

                        this.Id = id;
                        this.ClinicId = clinic.Id;
                        this.PractitionerId = practitioner.Id;
                        this.TreatmentId = treatment.Id;
                        this.Timeslot = timeslot;
                }
        }
}



