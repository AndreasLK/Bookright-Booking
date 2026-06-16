using System;

namespace Facade.Calendar
{
        public record CalendarEventViewModel(
            Guid Id,
            string Title,
            DateTime Start,
            DateTime End,
            string BackgroundColor,
            string TextColor
        );
}
