using Domain.Entities;
using Domain.Value_Objects;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Domain.Specifications
{
        /// <summary>
        /// Specification to find any existing bookings that overlap with a requested timeslot at a specific clinic.
        /// </summary>
        public class OverlappingBookingsSpecification : Specification<Booking>
        {
                private readonly Guid _clinicId;
                private readonly TimeSlot _requestedTimeSlot;

                public OverlappingBookingsSpecification(Guid clinicId, TimeSlot requestedTimeSlot)
                {
                        if (requestedTimeSlot is null)
                        {
                                throw new ArgumentNullException(paramName: nameof(requestedTimeSlot));
                        }

                        this._clinicId = clinicId;
                        this._requestedTimeSlot = requestedTimeSlot;
                }

                public override Expression<Func<Booking, bool>> ToExpression()
                {
                        return booking =>
                            booking.ClinicId.Value == this._clinicId &&
                            booking.Timeslot.StartDateTime < this._requestedTimeSlot.EndDateTime &&
                            booking.Timeslot.EndDateTime > this._requestedTimeSlot.StartDateTime;
                }
        }
}
