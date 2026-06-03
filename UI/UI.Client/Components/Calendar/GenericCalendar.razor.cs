using Microsoft.AspNetCore.Components;

namespace UI.Client.Components.Calendar
{
        /// <summary>
        /// A generic, dark-mode calendar component supporting variable resource columns and booking previews.
        /// </summary>
        public partial class GenericCalendar<TItem> : ComponentBase
        {
                [Parameter] public int StartHour { get; set; } = 8;
                [Parameter] public int EndHour { get; set; } = 18;

                [Parameter] public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();
                [Parameter] public IEnumerable<CalendarColumn> Columns { get; set; } = Array.Empty<CalendarColumn>();

                [Parameter] public TimeSelector<TItem> StartTimeSelector { get; set; } = default!;
                [Parameter] public TimeSelector<TItem> EndTimeSelector { get; set; } = default!;
                [Parameter] public ColorSelector<TItem> ColorSelector { get; set; } = default!;
                [Parameter] public ColumnSelector<TItem> ColumnIdSelector { get; set; } = default!;

                [Parameter] public TimeSpan? PendingBookingDuration { get; set; }
                [Parameter] public RenderFragment<TItem>? ItemTemplate { get; set; }

                [Parameter] public EventCallback<TItem> OnItemClicked { get; set; }
                [Parameter] public EventCallback<(DateTime Time, string ColumnId)> OnEmptySlotClicked { get; set; }

                private DateTime? _hoveredTime;
                private string? _hoveredColumnId;

                /// <summary>
                /// Filters the main item list to return only items belonging to a specific column.
                /// </summary>
                /// <param name="columnId">The unique identifier of the column.</param>
                /// <returns>A collection of items belonging to the column.</returns>
                private IEnumerable<TItem> GetItemsForColumn(string columnId)
                {
                        if (string.IsNullOrWhiteSpace(value: columnId))
                        {
                                return Array.Empty<TItem>();
                        }

                        return this.Items.Where(predicate: item => this.ColumnIdSelector(item: item) == columnId);
                }

                /// <summary>
                /// Updates the internal state to reflect where the user's mouse currently is.
                /// </summary>
                /// <param name="time">The time slot the mouse is hovering over.</param>
                /// <param name="columnId">The column the mouse is in.</param>
                private void HandleSlotHover(DateTime time, string columnId)
                {
                        this._hoveredTime = time;
                        this._hoveredColumnId = columnId;
                }

                /// <summary>
                /// Clears the hover state, removing the ghost booking preview.
                /// </summary>
                private void ClearHover()
                {
                        this._hoveredTime = null;
                        this._hoveredColumnId = null;
                }

                /// <summary>
                /// Calculates the dynamic CSS for rendering an event based on start and end time.
                /// </summary>
                /// <param name="item">The event item to render.</param>
                /// <returns>A CSS style string for absolute positioning.</returns>
                private string GetItemStyle(TItem item)
                {
                        if (item == null)
                        {
                                return string.Empty;
                        }

                        var start = this.StartTimeSelector(item: item);
                        var end = this.EndTimeSelector(item: item);
                        var color = this.ColorSelector != null
                            ? this.ColorSelector(item: item)
                            : CalendarConfig.DEFAULT_EVENT_COLOR;

                        return this.CalculatePositionStyle(startTime: start, endTime: end, backgroundColor: color);
                }

                /// <summary>
                /// Calculates the dynamic CSS for the semi-transparent ghost booking on hover.
                /// </summary>
                /// <returns>A CSS style string for absolute positioning of the preview.</returns>
                private string GetGhostStyle()
                {
                        if (this._hoveredTime.HasValue == false || this.PendingBookingDuration.HasValue == false)
                        {
                                return string.Empty;
                        }

                        var start = this._hoveredTime.Value;
                        var end = start.Add(value: this.PendingBookingDuration.Value);

                        // Uses a generic white overlay for the preview with 20% opacity.
                        return this.CalculatePositionStyle(startTime: start, endTime: end, backgroundColor: "rgba(255, 255, 255, 0.2)");
                }

                /// <summary>
                /// Calculates the top offset and height percentage for absolutely positioning elements on the grid.
                /// </summary>
                private string CalculatePositionStyle(DateTime startTime, DateTime endTime, string backgroundColor)
                {
                        var totalHours = (double)(this.EndHour - this.StartHour);

                        var startOffsetHours = startTime.TimeOfDay.TotalHours - this.StartHour;
                        var topPercentage = (startOffsetHours / totalHours) * CalendarConfig.MAX_PERCENTAGE;

                        var durationHours = (endTime - startTime).TotalHours;
                        var heightPercentage = (durationHours / totalHours) * CalendarConfig.MAX_PERCENTAGE;

                        // Clamp values using Math to prevent rendering outside the box (Unhappy path guard)
                        topPercentage = Math.Max(val1: 0, val2: topPercentage);
                        heightPercentage = Math.Min(val1: CalendarConfig.MAX_PERCENTAGE - topPercentage, val2: heightPercentage);

                        return $"top: {topPercentage:F2}%; height: {heightPercentage:F2}%; background-color: {backgroundColor};";
                }
        }
}
