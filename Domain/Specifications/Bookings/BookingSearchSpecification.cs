using Domain.Enums;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;

namespace Domain.Specifications.Bookings
{
        public class BookingSearchSpecification : Specification<Booking>
        {
                private readonly Guid? _customerId;
                private readonly Guid? _clinicId;
                private readonly Guid? _roomId;
                private readonly Guid? _practitionerId;


                public BookingSearchSpecification(
                        Guid? customerId = null,
                        Guid? clinicId = null,
                        Guid? roomId = null,
                        Guid? practitionerId = null,
                        BookingSortOption sortOption = BookingSortOption.StartTime,
                        SortDirection sortDirection = SortDirection.Ascending,
                        int skip = 0,
                        int take = 100
                    )
                {
                        this._customerId = customerId;
                        this._clinicId = clinicId;
                        this._roomId = roomId;
                        this._practitionerId = practitionerId;

                        this.ApplyPaging(skip: skip, take: take);

                        Expression<Func<Booking, object>> sortExpression = sortOption switch
                        {
                                BookingSortOption.CreatedAt => booking => booking.CreatedAt,
                                BookingSortOption.StartTime => booking => booking.Timeslot.StartDateTime,
                                _ => throw new ArgumentException(message: "Invalid sort option.", paramName: nameof(sortOption))
                        };

                        if (sortDirection == SortDirection.Ascending)
                        {
                                this.ApplyOrderBy(orderByExpression: sortExpression);
                        }
                        else
                        {
                                this.ApplyOrderByDescending(orderByDescendingExpression: sortExpression);
                        }

                }

                public override Expression<Func<Booking, bool>> ToExpression()
                {
                        return b =>
                                (!this._customerId.HasValue || b.CustomerId.Value == this._customerId.Value) &&
                                (!this._clinicId.HasValue || b.ClinicId.Value == this._clinicId.Value) &&
                                (!this._practitionerId.HasValue || b.PractitionerId.Value == this._practitionerId.Value) &&
                                (!this._roomId.HasValue || b.RoomId.Value == this._roomId.Value);
                }
        }
}
