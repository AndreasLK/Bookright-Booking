using Domain.Entities;
using Domain.Value_Objects;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Specifications.Bookings
{
        /// <summary>
        /// Specification to retrieve bookings scheduled in a specific set of rooms.
        /// </summary>
        public class BookingsByRoomIdsSpecification : Specification<Booking>
        {
                private readonly List<RoomId> _roomIds;

                public BookingsByRoomIdsSpecification(List<RoomId> roomIds)
                {
                        this._roomIds = roomIds;
                }

                public override Expression<Func<Booking, bool>> ToExpression()
                {
                        return booking => this._roomIds.Contains(booking.RoomId);
                }
        }
}
