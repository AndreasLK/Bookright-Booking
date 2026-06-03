namespace UI.Client.Components.Calendar
{
        /// <summary>
        /// Contains constant configuration values for the calendar component.
        /// </summary>
        public static class CalendarConfig
        {
                public const int MINUTES_IN_HOUR = 60;
                public const double MAX_PERCENTAGE = 100.0;
                public const int SLOT_INTERVAL_MINUTES = 15;
                public const string DEFAULT_EVENT_COLOR = "#0d6efd";
        }

        /// <summary>
        /// Custom delegates to enforce named arguments when extracting item data.
        /// </summary>
        public delegate DateTime TimeSelector<TItem>(TItem item);
        public delegate string ColorSelector<TItem>(TItem item);
        public delegate string ColumnSelector<TItem>(TItem item);

        /// <summary>
        /// Represents a generic vertical column in the calendar. 
        /// Can represent a Clinic, a Practitioner, or a Room on a specific date.
        /// </summary>
        public record CalendarColumn(string ColumnId, string Title, string Subtitle, DateTime Date);
}
