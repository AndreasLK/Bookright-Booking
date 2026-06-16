using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Specifications
{
        /// <summary>
        /// Specification to retrieve rooms that belong to the clinic and are not currently booked.
        /// </summary>
        public class AvailableRoomsSpecification : Specification<Room>
        {
                private readonly IReadOnlyList<Guid> _availableRoomIds;

                public AvailableRoomsSpecification(IEnumerable<Guid> availableRoomIds)
                {
                        this._availableRoomIds = availableRoomIds?.ToList() ?? new List<Guid>();
                }

                public override Expression<Func<Room, bool>> ToExpression()
                {
                        return room => this._availableRoomIds.Contains(room.Id.Value);
                }
        }
}
