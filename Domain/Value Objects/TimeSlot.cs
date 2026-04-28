namespace Domain.Value_Objects
{
        /// <summary>
        /// Represents a specific temporal interval with a fixed start and end point.
        /// </summary>
        /// <remarks>
        /// Enforces temporal integrity by ensuring the start time always precedes the end time.
        /// </remarks>
        public record TimeSlot
        {
                /// <summary>The beginning of the reserved time slot.</summary>
                public DateTimeOffset StartDateTime { get; init; }


                /// <summary>The end of the reserved time slot.</summary>
                public DateTimeOffset EndDateTime { get; init; }


                /// <summary>
                /// Initializes a new instance of the <see cref="TimeSlot"/> record.
                /// Validates that the start time occurs before the end time.
                /// </summary>
                /// <param name="startDateTime">The start of the interval.</param>
                /// <param name="endDateTime">The end of the interval.</param>
                public TimeSlot(DateTimeOffset startDateTime, DateTimeOffset endDateTime)
                {
                        if (startDateTime >= endDateTime)
                        {
                                throw new ArgumentException("The Start time must be before the End time");
                        }

                        this.StartDateTime = startDateTime;
                        this.EndDateTime = endDateTime;

                }

                /// <summary>
                /// Determines whether this time slot overlaps with another specified interval.
                /// </summary>
                /// <param name="other">The other time slot to compare against.</param>
                /// <returns>True if the intervals intersect; otherwise, false.</returns>
                public bool Overlaps(TimeSlot other)
                {
                        return this.StartDateTime < other.EndDateTime && other.StartDateTime < this.EndDateTime;
                }
        }
}
