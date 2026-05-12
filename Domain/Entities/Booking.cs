
using Domain.Entities.Persons;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System;
using System.Collections.Generic;
using System.Text;

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

                public CustomerId CustomerId { get; private set; }

                public RoomId RoomId { get; private set; }

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

                public DateTime CreatedAt { get; private set; }

                public Money? Paid { get; private set; }

                public CampaignId? AppliedCampaign { get; private set; }

                public Booking(BookingId id,
                                ClinicId clinic,
                                PractitionerId practitioner,
                                TreatmentId treatment,
                                RoomId room,
                                CustomerId customer,
                                TimeSlot timeslot,
                                DateTime? createdAt = null,
                                Money? paid = null,
                                CampaignId? appliedCampaign = null)

                {
                        ArgumentNullException.ThrowIfNull(argument: id, paramName: nameof(id));
                        ArgumentNullException.ThrowIfNull(argument: clinic, paramName: nameof(clinic));
                        ArgumentNullException.ThrowIfNull(argument: practitioner, paramName: nameof(practitioner));
                        ArgumentNullException.ThrowIfNull(argument: treatment, paramName: nameof(treatment));
                        ArgumentNullException.ThrowIfNull(argument: room, paramName: nameof(room));
                        ArgumentNullException.ThrowIfNull(argument: timeslot, paramName: nameof(timeslot));


                        if (createdAt is null)
                        {
                                createdAt = DateTime.Now;
                        }

                        this.Id = id;
                        this.PractitionerId = practitioner;
                        this.TreatmentId = treatment;
                        this.ClinicId = clinic;
                        this.RoomId = room;
                        this.Timeslot = timeslot;
                        this.CreatedAt = createdAt.Value;
                        this.Paid = paid;
                        this.AppliedCampaign = appliedCampaign;

                }
        }
}
