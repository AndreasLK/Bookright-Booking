using Domain.Enums;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;

namespace Domain.Specifications
{
        /// <summary>
        /// Specification to filter bookings specifically for the calendar view.
        /// Supports multiple selections (IN clauses) for Clinics, Rooms, Practitioners, and Customers.
        /// Ensures bookings overlap with the currently visible calendar date range.
        /// </summary>
        public class CalendarBookingSpecification : Specification<Booking>
        {
                private readonly DateTime _viewStartDate;
                private readonly DateTime _viewEndDate;

                private readonly IReadOnlyList<Guid> _clinicIds;
                private readonly IReadOnlyList<Guid> _roomIds;
                private readonly IReadOnlyList<Guid> _practitionerIds;
                private readonly IReadOnlyList<Guid> _customerIds;

                /// <summary>
                /// Initializes a new instance of the <see cref="CalendarBookingSpecification"/> class.
                /// </summary>
                /// <param name="viewStartDate">The start date of the visible calendar.</param>
                /// <param name="viewEndDate">The end date of the visible calendar.</param>
                /// <param name="clinicIds">Collection of selected clinic IDs.</param>
                /// <param name="roomIds">Collection of selected room IDs.</param>
                /// <param name="practitionerIds">Collection of selected practitioner IDs.</param>
                /// <param name="customerIds">Collection of selected customer IDs.</param>
                public CalendarBookingSpecification(
                    DateTime viewStartDate,
                    DateTime viewEndDate,
                    IEnumerable<Guid>? clinicIds = null,
                    IEnumerable<Guid>? roomIds = null,
                    IEnumerable<Guid>? practitionerIds = null,
                    IEnumerable<Guid>? customerIds = null)
                {
                        this._viewStartDate = viewStartDate;
                        this._viewEndDate = viewEndDate;

                        // Default to empty lists if null is passed, maintaining strict state
                        this._clinicIds = clinicIds?.ToList() ?? new List<Guid>();
                        this._roomIds = roomIds?.ToList() ?? new List<Guid>();
                        this._practitionerIds = practitionerIds?.ToList() ?? new List<Guid>();
                        this._customerIds = customerIds?.ToList() ?? new List<Guid>();
                }

                /// <summary>
                /// Translates the multiple-selection calendar rules into a LINQ expression for Entity Framework.
                /// </summary>
                /// <returns>An expression evaluating to true if the booking belongs on the calendar.</returns>
                public override Expression<Func<Booking, bool>> ToExpression()
                {
                        return booking =>
                            // Time Overlap Check: The booking must fall within the visible calendar window
                            booking.Timeslot.StartDateTime < this._viewEndDate &&
                            booking.Timeslot.EndDateTime > this._viewStartDate &&

                            // Multi-select IN clauses (If the filter list has items, the booking's ID MUST be in that list)
                            (!this._clinicIds.Any() || this._clinicIds.Contains(booking.ClinicId.Value)) &&
                            (!this._roomIds.Any() || this._roomIds.Contains(booking.RoomId.Value)) &&
                            (!this._practitionerIds.Any() || this._practitionerIds.Contains(booking.PractitionerId.Value)) &&
                            (!this._customerIds.Any() || this._customerIds.Contains(booking.CustomerId.Value));
                }
        }
}
