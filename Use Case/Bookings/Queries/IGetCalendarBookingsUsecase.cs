using System;
using System.Collections.Generic;
using System.Text;

namespace Use_Case.Bookings.Queries
{
        /// <summary>
        /// Defines the contract for fetching raw calendar bookings based on a filter.
        /// </summary>
        public interface IGetCalendarBookingsUseCase
        {
                /// <summary>
                /// Executes the query to fetch raw calendar bookings.
                /// </summary>
                /// <param name="filter">The filter criteria for the calendar.</param>
                /// <returns>A collection of raw data DTOs.</returns>
                public Task<IEnumerable<CalendarBookingResultDto>> ExecuteAsync(CalendarBookingFilter filter);
        }
}
