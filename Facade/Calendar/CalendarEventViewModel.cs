using System;
using System.Collections.Generic;
using System.Text;

namespace Facade.Calendar
{
        public record CalendarEventViewModel(
            Guid Id,
            string Title,
            DateTime Start,
            DateTime End,
            string BackgroundColor
        );
}
