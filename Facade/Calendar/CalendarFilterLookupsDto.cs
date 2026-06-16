using Facade.Clinics;
using Facade.Common.Dtos;
using Facade.Customers;
using Facade.Rooms;
using System.Collections.Generic;

namespace Facade.Calendar
{
        public record CalendarFilterLookupsDto(
            IEnumerable<ClinicDto> Clinics,
            IEnumerable<RoomDto> Rooms,
            IEnumerable<PractitionerLookupDto> Practitioners,
            IEnumerable<CustomerSummaryDto> Customers,
            IEnumerable<TreatmentLookupDto> Treatments
        );
}
