using Facade.Rooms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Facade.Clinics
{
        /// <summary>
        /// Represents a clinic and its available rooms for the dashboard sidebar.
        /// </summary>
        public record ClinicSummaryDto(Guid Id, string Name, List<RoomSummaryDto> Rooms);
}
