using Facade.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Facade.Rooms
{
        /// <summary>
        /// Data Transfer Object representing a room for UI selection and filtering.
        /// </summary>
        public record RoomDto
        {
                /// <summary>
                /// Unique identifier for the room.
                /// </summary>
                public Guid Id { get; init; }

                /// <summary>
                /// The display name of the room.
                /// </summary>
                [Searchable]
                public string Name { get; init; } = string.Empty;
        }
}
