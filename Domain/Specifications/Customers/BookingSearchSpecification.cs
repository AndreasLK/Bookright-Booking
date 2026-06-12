using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Specifications.Customers
{
        /// <summary>
        /// Specification to filter, sort, and paginate bookings for list and table views.
        /// </summary>
        public class BookingSearchSpecification : Specification<Booking>
        {
                private readonly Guid? _customerId;
                private readonly Guid? _clinicId;
                private readonly Guid? _roomId;
                private readonly Guid? _practitionerId;

                /// <summary>
                /// Initializes a new instance of the BookingSearchSpecification.
                /// </summary>
                public BookingSearchSpecification(
                    Guid? customerId = null,
                    Guid? clinicId = null,
                    Guid? roomId = null,
                    Guid? practitionerId = null,
                    BookingSortOption sortOption = BookingSortOption.StartTime,
                    SortDirection sortDirection = SortDirection.Ascending,
                    int skip = 0,
                    int take = 100)
                {
                        this._customerId = customerId;
                        this._clinicId = clinicId;
                        this._roomId = roomId;
                        this._practitionerId = practitionerId;

                        // Apply Pagination
                        this.ApplyPaging(skip: skip, take: take);

                        // Determine Sorting Expression
                        Expression<Func<Booking, object>> sortExpression = sortOption switch
                        {
                                BookingSortOption.CreatedAt => booking => booking.CreatedAt,
                                BookingSortOption.StartTime => booking => booking.Timeslot.StartDateTime,
                                _ => throw new ArgumentException(message: "Invalid sort option.", paramName: nameof(sortOption))
                        };

                        // Apply Sorting Direction
                        if (sortDirection == SortDirection.Ascending)
                        {
                                this.ApplyOrderBy(orderByExpression: sortExpression);
                        }
                        else
                        {
                                this.ApplyOrderByDescending(orderByDescendingExpression: sortExpression);
                        }
                }

                /// <summary>
                /// Translates the filtering rules into a LINQ expression.
                /// </summary>
                public override Expression<Func<Booking, bool>> ToExpression()
                {
                        return booking =>
                            (!this._customerId.HasValue || booking.CustomerId.Value == this._customerId.Value) &&
                            (!this._clinicId.HasValue || booking.ClinicId.Value == this._clinicId.Value) &&
                            (!this._practitionerId.HasValue || booking.PractitionerId.Value == this._practitionerId.Value) &&
                            (!this._roomId.HasValue || booking.RoomId.Value == this._roomId.Value);
                }
        }
}
