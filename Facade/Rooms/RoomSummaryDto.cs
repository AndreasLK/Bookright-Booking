using System;
using System.Collections.Generic;
using System.Text;

namespace Facade.Rooms
{
        /// <summary>
        /// Represents a room within a clinic for UI rendering.
        /// </summary>
        public record RoomSummaryDto(Guid Id, string Name);
}
