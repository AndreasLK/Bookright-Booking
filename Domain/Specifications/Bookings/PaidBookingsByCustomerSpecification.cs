using Domain.Entities;
using Domain.Value_Objects.Ids;
using System.Linq.Expressions;

namespace Domain.Specifications
{
        public class PaidBookingsByCustomerSpecification : Specification<Booking>
        {
                private readonly CustomerId _customerId;
                private readonly DateTime _sinceDate;
                private readonly DateTime _untilDate;

                public PaidBookingsByCustomerSpecification(CustomerId customerId, DateTime bookingsAfter, DateTime bookingsBefore)
                {
                        ArgumentNullException.ThrowIfNull(argument: customerId, nameof(customerId));
                        this._customerId = customerId;
                        this._untilDate = bookingsBefore;
                        this._sinceDate = bookingsAfter;
                }

                public override Expression<Func<Booking, bool>> ToExpression()
                {
                        return booking =>
                        booking.CustomerId == this._customerId &&
                        booking.Paid != null &&
                        booking.Timeslot.StartDateTime >= this._sinceDate &&
                        booking.Timeslot.EndDateTime <= this._untilDate;
                }


        }
}
