using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Use_Case.Bookings.Queries;

namespace Facade.Calendar
{
        public interface ICalendarFacade
        {
                public Task<IEnumerable<CalendarEventViewModel>> GetCalendarEventsAsync(CalendarBookingFilter filter);
                public Task<CalendarFilterLookupsDto> GetFilterLookupsAsync();

                public Task<IEnumerable<CalendarEventViewModel>> RefreshCalendarBookingsAsync(
                    DateTime viewStartDate,
                    DateTime viewEndDate,
                    List<Guid> clinicIds,
                    List<Guid> roomIds,
                    List<Guid> practitionerIds,
                    List<Guid> customerIds
                );
        }
}
