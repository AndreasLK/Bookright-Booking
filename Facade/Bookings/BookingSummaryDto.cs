using System;
using Domain.Enums;
using Facade.Common.Attributes;

namespace Facade.Bookings
{
        /// <summary>
        /// Summary of a booking for UI presentation.
        /// </summary>
        public class BookingSummaryDto
        {
                /// <summary>
                /// Unique identifier.
                /// </summary>
                public Guid Id { get; set; }

                /// <summary>
                /// Associated customer identifier.
                /// </summary>
                public Guid CustomerId { get; set; }

                /// <summary>
                /// Name of the scheduled treatment.
                /// </summary>
                [Searchable]
                public string TreatmentName { get; set; } = string.Empty;

                /// <summary>
                /// Display name of the assigned practitioner.
                /// </summary>
                [Searchable]
                public string PractitionerName { get; set; } = string.Empty;

                /// <summary>
                /// Unique identifier of the assigned practitioner.
                /// </summary>
                public Guid PractitionerId { get; set; }

                /// <summary>
                /// Name of the clinic where the treatment occurs.
                /// </summary>
                [Searchable]
                public string ClinicName { get; set; } = string.Empty;

                /// <summary>
                /// Display name of the customer.
                /// </summary>
                [Searchable]
                public string CustomerName { get; set; } = string.Empty;

                /// <summary>
                /// Start time of the appointment.
                /// </summary>
                public DateTime StartTime { get; set; }

                /// <summary>
                /// End time of the appointment.
                /// </summary>
                public DateTime EndTime { get; set; }

                /// <summary>
                /// Monetary amount paid, or null if unpaid.
                /// </summary>
                public decimal? AmountPaid { get; set; }

                /// <summary>
                /// Formatted date string for UI display.
                /// </summary>
                public string BookingDateFormatted => this.StartTime.ToString("d MMM yyyy");

                /// <summary>
                /// Formatted time range string for UI display.
                /// </summary>
                public string BookingTimeFormatted => $"{this.StartTime:HH:mm} - {this.EndTime:HH:mm}";


                /// <summary>
                /// The current status of the booking, derived from payment state and timeslot.
                /// </summary>
                public BookingStatus Status { get; set; }

                /// <summary>
                /// Status string for UI display.
                /// </summary>
                public string StatusFormatted => this.Status switch
                {
                        BookingStatus.Scheduled => "Planlagt",
                        BookingStatus.Paid => "Betalt",
                        BookingStatus.Completed => "Gennemført",
                        BookingStatus.NoShow => "Udeblevet",
                        _ => "Ukendt"
                };
        }
}
