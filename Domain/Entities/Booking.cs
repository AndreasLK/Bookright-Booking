
using Domain.Entities.Persons;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

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
                /// Identifier of the customer who made the booking.
                /// </summary>
                public CustomerId CustomerId { get; private set; }

                /// <summary>
                /// Identifier of the room where the treatment takes place.
                /// </summary>
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

                /// <summary>
                /// Timestamp indicating exactly when the booking was created.
                /// </summary>
                public DateTime CreatedAt { get; private set; }

                /// <summary>
                /// Monetary amount paid for the booking, or null if it is currently unpaid.
                /// </summary>
                public Money? Paid { get; private set; }

                /// <summary>
                /// Identifier of any promotional campaign applied to reduce the booking price.
                /// </summary>
                public CampaignId? AppliedCampaign { get; private set; }

                /// <summary>
                /// Initializes a new instance of the <see cref="Booking"/> class.
                /// </summary>
                /// <param name="id">Unique booking identifier.</param>
                /// <param name="clinic">Associated clinic identifier.</param>
                /// <param name="practitioner">Assigned practitioner identifier.</param>
                /// <param name="treatment">Selected treatment identifier.</param>
                /// <param name="room">Identifier of the room.</param>
                /// <param name="customer">Identifier of the customer.</param>
                /// <param name="timeslot">Allocated time window for the appointment.</param>
                /// <param name="createdAt">Optional creation timestamp. Defaults to the current time if null.</param>
                /// <param name="paid">Optional monetary amount paid.</param>
                /// <param name="appliedCampaign">Optional applied promotional campaign identifier.</param>
                /// <exception cref="ArgumentNullException">Thrown if any required identifier or the timeslot is null.</exception>
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
                        ArgumentNullException.ThrowIfNull(argument: customer, paramName: nameof(customer));
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
                        this.CustomerId = customer;
                        this.Timeslot = timeslot;
                        this.CreatedAt = createdAt.Value;
                        this.Paid = paid;
                        this.AppliedCampaign = appliedCampaign;

                }

                /// <summary>
                /// Updates the timeslot, ensuring domain rules are met.
                /// </summary>
                /// <param name="newTimeslot">The newly requested time window.</param>
                public void Reschedule(TimeSlot newTimeslot)
                {
                        // Example of a Domain Rule: Cannot reschedule to a past date
                        if (newTimeslot.StartDateTime < DateTime.Now)
                        {
                                throw new InvalidOperationException("Cannot reschedule a booking to the past.");
                        }

                        this.Timeslot = newTimeslot;
                }

                /// <summary>
                /// Registers a payment amount.
                /// </summary>
                /// <param name="payment">The monetary amount provided.</param>
                public void RegisterPayment(Money payment)
                {
                        if (payment.Value < 0)
                        {
                                throw new ArgumentException("Payment amount cannot be negative.");
                        }

                        this.Paid = payment;
                }

                /// <summary>
                /// Reassigns the appointment to a different practitioner.
                /// </summary>
                /// <param name="newPractitionerId">The new practitioner identifier.</param>
                /// <exception cref="InvalidOperationException">Thrown when attempting to change a past booking.</exception>
                public void ReassignPractitioner(PractitionerId newPractitionerId)
                {
                        ArgumentNullException.ThrowIfNull(argument: newPractitionerId, paramName: nameof(newPractitionerId));

                        if (this.Timeslot.StartDateTime < DateTime.Now)
                        {
                                throw new InvalidOperationException(message: "Cannot reassign a practitioner for a booking in the past.");
                        }

                        this.PractitionerId = newPractitionerId;
                }

                /// <summary>
                /// The current status of the booking, derived from payment state and timeslot.
                /// </summary>
                public BookingStatus Status
                {
                        get
                        {
                                if (this.Paid != null && this.Timeslot.StartDateTime > DateTime.Now)
                                        return BookingStatus.Paid;

                                if (this.Paid != null && this.Timeslot.StartDateTime <= DateTime.Now)
                                        return BookingStatus.Completed;

                                if (this.Paid == null && this.Timeslot.StartDateTime <= DateTime.Now)
                                        return BookingStatus.NoShow;

                                return BookingStatus.Scheduled;
                        }
                }
        }
}
