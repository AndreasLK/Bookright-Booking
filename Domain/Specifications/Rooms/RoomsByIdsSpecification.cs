using Domain.Entities;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Specifications.Rooms
{
        /// <summary>
        /// Specification to fetch concrete room entities matching a specific collection of unique room identifiers.
        /// </summary>
        public class RoomsByIdsSpecification : Specification<Room>
        {
                private readonly List<RoomId> _roomIds;

                /// <summary>
                /// Initializes a new instance of the <see cref="RoomsByIdsSpecification"/> class.
                /// </summary>
                /// <param name="roomIds">The collection of target room identifiers.</param>
                public RoomsByIdsSpecification(List<RoomId> roomIds)
                {
                        this._roomIds = roomIds;
                }

                /// <inheritdoc />
                public override Expression<Func<Room, bool>> ToExpression()
                {
                        return room => this._roomIds.Contains(item: room.Id);
                }
        }
}
