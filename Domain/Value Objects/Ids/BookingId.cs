using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Value_Objects
{
        public record BookingId
        {
                public Guid Value { get; init; }

                public BookingId(Guid value)
                {
                        if (value == Guid.Empty)
                        {
                                throw new ArgumentException(
                                        message: "ID must not be empty",
                                        paramName: nameof(value));
                        }
                        this.Value = value;

                }
        }
}
