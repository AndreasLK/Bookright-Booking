using Domain.Enums;

namespace Domain.Value_Objects
{
        /// <summary>
        /// Defines the operating window for a clinic on a specific day.
        /// </summary>
        public record OpeningHour
        {
                /// <summary>
                /// The specific day of the week.
                /// </summary>
                public Weekday Day { get; init; }

                /// <summary>
                /// The time the clinic opens for appointments.
                /// </summary>
                public TimeOnly OpenAt { get; init; }

                /// <summary>
                /// The time the clinic closes for appointments.
                /// </summary>
                public TimeOnly CloseAt { get; init; }

                /// <summary>
                /// Initializes a new instance of the <see cref="OpeningHour"/> record.
                /// </summary>
                /// <param name="day">Day of the week.</param>
                /// <param name="openAt">Opening time.</param>
                /// <param name="closeAt">Closing time.</param>
                /// <exception cref="ArgumentException">Thrown if closing time is not after opening time.</exception>
                public OpeningHour(Weekday day, TimeOnly openAt, TimeOnly closeAt)
                {
                        if (closeAt <= openAt)
                        {
                                throw new ArgumentException(
                                    message: "Closing time must be after opening time.",
                                    paramName: nameof(closeAt));
                        }

                        this.Day = day;
                        this.OpenAt = openAt;
                        this.CloseAt = closeAt;
                }
        }
}
