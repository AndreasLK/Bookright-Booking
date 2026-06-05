using System;
using System.Collections.Generic;
using System.Text;

namespace Facade.Treatments
{
        /// <summary>
        /// Represents a treatment available for booking.
        /// </summary>
        public record TreatmentSummaryDto(Guid Id, string Name, int DurationMinutes);
}
