using Facade.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Facade.Bookings
{
        public class BookingSummaryDto
        {
                public Guid Id { get; set; }

                public Guid CustomerId { get; set; }

                [Searchable]
                public string TreatmentName { get; set; } = string.Empty;

                [Searchable]
                public string PractitionerName { get; set; } = string.Empty;

                public Guid PractitionerId { get; set; }

                [Searchable]
                public string ClinicName { get; set; } = string.Empty;

                [Searchable]
                public string CustomerName { get; set; } = string.Empty;

                public DateTime StartTime { get; set; }
                public DateTime EndTime { get; set; }

                public decimal? AmountPaid { get; set; }

                // Helpful formatted properties for the Razor view
                public string BookingDateFormatted => this.StartTime.ToString("d MMM yyyy");
                public string BookingTimeFormatted => $"{this.StartTime:HH:mm} - {this.EndTime:HH:mm}";
                public string PaymentStatus => this.AmountPaid.HasValue ? "Betalt" : "Mangler Betaling";
        }
}
